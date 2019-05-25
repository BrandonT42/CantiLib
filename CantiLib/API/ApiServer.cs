//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Canti
{
    // This is used to label api methods in our source
    public sealed class ApiMethod : Attribute
    {
        public string MethodName;
        public ApiMethod(string MethodName)
        {
            this.MethodName = MethodName.ToUpper();
        }
    }

    // A self-contained rest API server/listener
    public sealed class ApiServer
    {
        #region Properties and Fields

        public Logger Logger { get; set; }
        public int Port { get; private set; }

        private IMethodContext MethodContext { get; set; }
        private HttpListener Listener { get; set; }
        private Thread ListenerThread { get; set; }
        private Thread[] WorkerThreads { get; set; }
        private ManualResetEvent StopEvent { get; set; }
        private ManualResetEvent ReadyEvent { get; set; }
        private Queue<HttpListenerContext> ContextQueue { get; set; }

        #endregion

        #region Methods

        #region Internal

        // Starts our HTTP listener and worker threads
        internal void Start(int Port)
        {
            // Store port
            this.Port = Port;

            // Start HTTP listener
            try
            {
                Listener = new HttpListener();
                Listener.Prefixes.Add($"http://+:{Port}/");
                Listener.Start();
            }
            catch
            {
                // TODO - better error handling
                Listener = new HttpListener();
                Listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
                Listener.Start();
            }

            // Begin listener thread
            ListenerThread.Start();

            // Start our worker threads
            for (int i = 0; i < WorkerThreads.Length; i++)
            {
                WorkerThreads[i] = new Thread(AcceptContext);
                WorkerThreads[i].Start();
            }
        }

        // Stops the API server and ends all associated threads
        internal void Stop()
        {
            StopEvent.Set();
            ListenerThread.Join();
            foreach (Thread worker in WorkerThreads) worker.Join();
            Listener.Stop();
        }

        // Assigns our method context in which we will invoke methods
        internal void AssignMethodContext(IMethodContext Context)
        {
            MethodContext = Context;
        }

        #endregion

        #region Private

        // Listens for new requests and enqueues them
        public void Listen()
        {
            while (Listener.IsListening)
            {
                // Accept new connection context
                var Context = Listener.BeginGetContext((IAsyncResult Result) =>
                {
                    try
                    {
                        // Lock our context queue to prevent race conditions
                        lock (ContextQueue)
                        {
                            // Add new connection context to our context queue
                            ContextQueue.Enqueue(Listener.EndGetContext(Result));

                            // Signal that a context is ready to be accepted
                            ReadyEvent.Set();
                        }
                    }
                    catch { }
                }, null);

                // Wait for exit
                if (WaitHandle.WaitAny(new[] { StopEvent, Context.AsyncWaitHandle }) == 0) return;
            }
        }

        // Accepts new connection contexts
        private void AcceptContext()
        {
            // Create a wait handle array so we can cancel this thread if need be
            WaitHandle[] wait = new[] { ReadyEvent, StopEvent };
            while (0 == WaitHandle.WaitAny(wait))
            {
                // Lock our context queue to prevent race conditions
                lock (ContextQueue)
                {
                    // Context queue has entries, accept one
                    if (ContextQueue.Count > 0)
                    {
                        // Dequeue next context in line
                        var Context = ContextQueue.Dequeue();

                        // Handle this context
                        HandleRequest(Context);
                    }

                    // There are no entries in the connection queue
                    else
                    {
                        // No context in line, reset ready event
                        ReadyEvent.Reset();
                        continue;
                    }
                }
            }
        }

        // Handles an incoming request
        private void HandleRequest(HttpListenerContext Context)
        {
            string Result = "";
            try
            {
                // Get request information
                var Request = Context.Request;
                string RequestType = Request.HttpMethod;

                // Check request's API version
                if (!int.TryParse(Request.Url.Segments[1].Replace("/", ""), out int Version) ||
                    !MethodContext.CheckVersion(Version))
                {
                    throw new InvalidOperationException("Invalid API version");
                }

                // Check if requested method exists
                string MethodName = Request.Url.Segments[2].Replace("/", "").ToUpper();
                if (MethodContext.GetType().GetMethods().Count(x => x.GetCustomAttributes(true)
                    .Any(i => i is ApiMethod && ((ApiMethod)i).MethodName == MethodName)) == 0)
                {
                    throw new InvalidOperationException("Invalid API method");
                }
                var Method = MethodContext.GetType().GetMethods().Where(x => x.GetCustomAttributes(true)
                    .Any(i => i is ApiMethod && ((ApiMethod)i).MethodName == MethodName)).First();

                // "GET" request
                if (RequestType == "GET")
                {
                    // Get our request's params directly from URL string
                    string[] Params = Request.Url.Segments.Skip(3).Select(x => x.Replace("/", "")).ToArray();
                    Logger?.Debug($"[{Request.RemoteEndPoint.Address.ToString()} API] " +
                        $"/{Version}/{MethodName}/{string.Join("/", Request.Url.Segments.Skip(3))}");

                    // Populate our parameters
                    var Parameters = Method.GetParameters();
                    object[] MethodParams = new object[Parameters.Length];
                    for (int i = 0; i < Parameters.Length; i++)
                    {
                        if (i < Params.Length) MethodParams[i] = Convert.ChangeType(Params[i], Parameters[i].ParameterType);
                        else MethodParams[i] = null;
                    }

                    // Invoke the requested method
                    Result = (string)Method.Invoke(MethodContext, MethodParams);
                }

                // "POST" request
                else if (RequestType == "POST")
                {
                    // Get our request's params from the stream data
                    string RequestBody = "";
                    using (var Reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
                        RequestBody = Reader.ReadToEnd();
                    JObject Params = JsonConvert.DeserializeObject<JObject>(RequestBody);
                    Logger?.Debug($"[{Request.RemoteEndPoint.Address.ToString()} API] /{Version}/{MethodName}/");

                    // Invoke the requested method
                    Result = (string)Method.Invoke(this, new object[] { Params });
                }
            }

            // A known error was caught
            catch (InvalidOperationException e)
            {
                Result = $"{{'error':'{e.Message}'}}";
            }

            // Unable to parse this request
            catch
            {
                Result = "{'error':'Invalid request'}";
            }

            // Populate a new HTTP header and send our response
            var Buffer = Encoding.UTF8.GetBytes(Result);
            var Response = Context.Response;
            Response.ContentLength64 = Buffer.Length;
            using (var st = Response.OutputStream)
                st.Write(Buffer, 0, Buffer.Length);
            Response.Close();
        }

        #endregion

        #endregion

        #region Constructors

        internal ApiServer(int MaxWorkers)
        {
            // Setup threads and events
            WorkerThreads = new Thread[MaxWorkers];
            ContextQueue = new Queue<HttpListenerContext>();
            StopEvent = new ManualResetEvent(false);
            ReadyEvent = new ManualResetEvent(false);
            ListenerThread = new Thread(Listen);
        }

        #endregion
    }
}

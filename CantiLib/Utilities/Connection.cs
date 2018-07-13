namespace Canti.Utilities
{
    public class Connection
    {
        internal string Host { get; private set; }
        internal string Endpoint{ get; private set; }
        internal string Password { get; private set; }
        internal int Port { get; private set; }
        public Connection(string Host, int Port, string Endpoint = "", string Password = "")
        {
            this.Host = Host;
            this.Port = Port;
            this.Endpoint = Endpoint;
            this.Password = Password;
        }
        internal string ToString(bool IgnoreEndpoint = false)
        {
            string Output = "http://" + Host + ":" + Port + "/";
            if (!IgnoreEndpoint) Output += Endpoint;
            return Output;
        }
    }
}

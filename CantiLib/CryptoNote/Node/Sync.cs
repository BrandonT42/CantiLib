using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Canti.CryptoNote
{
    public sealed partial class Node
    {
        #region Properties and Fields

        #region Private

        // A threading timer that will send a sync packet request every so often
        private Timer SyncTimer { get; set; }

        #endregion

        #endregion

        #region Methods

        // Begins the sync process
        private void StartSync()
        {
            // Start our sync timer
            SyncTimer = new Timer(new TimerCallback(Sync), null, 0, Globals.P2P_TIMED_SYNC_INTERVAL * 1000);
        }

        // Stops the sync process
        private void StopSync()
        {
            // Dispose of our timer, clearing resources
            SyncTimer.Dispose();
        }

        // Sends a timed sync request to all connected peers
        private void Sync(object State)
        {
            // Lock our peer list to prevent any race conditions
            lock (PeerList)
            {
                // Loop through all connected peers
                foreach (var Peer in PeerList)
                {
                    // Send this peer a timed sync request
                    TimedSync(Peer);
                }
            }
        }

        #endregion
    }
}

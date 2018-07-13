using System;

namespace Canti.Utilities
{
    internal class GeneralUtilities
    {
        // Returns the current unix timestamp
        public static ulong GetTimestamp()
        {
            return (ulong)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}

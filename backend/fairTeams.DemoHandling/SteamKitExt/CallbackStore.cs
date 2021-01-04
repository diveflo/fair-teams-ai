using SteamKit2.GC;
using System;
using System.Collections.Generic;

namespace fairTeams.DemoHandling.SteamKitExt
{
    internal class CallbackStore
    {
        private readonly Dictionary<uint, Queue<Action<IPacketGCMsg>>> myCallbacks = new();

        public bool TryGetValue(uint key, out Action<IPacketGCMsg> func)
        {
            if (myCallbacks.ContainsKey(key) && (myCallbacks[key].Count != 0))
            {
                func = myCallbacks[key].Dequeue();
                return true;
            }

            func = null;
            return false;
        }

        public void Add(uint key, Action<IPacketGCMsg> action)
        {
            if (!myCallbacks.ContainsKey(key))
                myCallbacks.Add(key, new Queue<Action<IPacketGCMsg>>());

            myCallbacks[key].Enqueue(action);
        }
    }
}
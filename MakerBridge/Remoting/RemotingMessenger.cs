using System;
using System.Collections.Generic;

namespace MakerBridge.Remoting
{
    class RemotingMessenger : MarshalByRefObject, IMessenger
    {
        private static Object lockObj = new Object();
        private Dictionary<byte, Queue<byte[]>> messages;

        RemotingMessenger()
        {
            messages = new Dictionary<byte, Queue<byte[]>>
            {
                { 0, new Queue<byte[]>() },
                { 1, new Queue<byte[]>() }
            };
        }

        public void SendMessage(byte type, byte[] message)
        {
            lock(lockObj)
            {
                messages[type].Enqueue(message);
            }
        }

        public byte[] GetMessage(byte type)
        {
            lock(lockObj)
            {
                return messages[type].Count > 0 ? messages[type].Dequeue() : null;  
            }
        }

        public void ClearMessage(byte type)
        {
            lock(lockObj)
            {
                messages[type].Clear();
            }
        }
    }
}

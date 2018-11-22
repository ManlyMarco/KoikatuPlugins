using System;
using System.Collections.Generic;

namespace MakerBridge.Remoting
{
    class RemotingMessenger : MarshalByRefObject, IMessenger
    {
        private static Object lockObj = new Object();
        private Queue<byte[]> messages = new Queue<byte[]>();

        public void SendMessage(byte[] message)
        {
            lock(lockObj)
            {
                messages.Enqueue(message);
            }
        }

        public byte[] GetMessage()
        {
            lock(lockObj)
            {
                return messages.Count > 0 ? messages.Dequeue() : null;  
            }
        }

        public void ClearMessage()
        {
            lock(lockObj)
            {
                messages.Clear();
            }
        }
    }
}

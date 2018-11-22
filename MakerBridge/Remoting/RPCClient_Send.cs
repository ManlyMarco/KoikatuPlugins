using System;
using MessagePack;

namespace MakerBridge.Remoting
{
    static class RPCClient_Send
    {
        private static IMessenger remoteObject;

        public static void Start(string name, int port)
        {
            Type requiredType = typeof(IMessenger);
            remoteObject = (IMessenger)Activator.GetObject(requiredType, $"tcp://localhost:{port}/{name}");
        }

        public static void SendMessage(MsgObject message)
        {
            var bytes = MessagePackSerializer.Serialize(message);
            remoteObject.SendMessage(bytes);
        }
    }
}

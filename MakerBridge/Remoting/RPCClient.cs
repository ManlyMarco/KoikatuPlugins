using System;
using System.Threading;
using MessagePack;
using static BepInEx.Logger;
using BepInEx.Logging;

namespace MakerBridge.Remoting
{
    static class RPCClient
    {
        private static IMessenger remoteObject;
        private static Action<MsgObject> messageAction;
        private static string serverName;
        private static int serverPort;
        private static byte listenId;
        private static byte sendId;
        private static volatile bool threadRunning = false;

        public static void Init(string name, int port, byte send, byte receive, Action<MsgObject> action)
        {
            messageAction = action;
            serverName = name;
            serverPort = port;
            listenId = send;
            sendId = receive;
        }

        public static void Listen()
        {
            if(!threadRunning)
            {
                new Thread(RefreshMessages).Start();
            }
            else
            {
                Console.WriteLine("[MakerBridge] Client already running");
            }
        }

        public static void SendOnly()
        {
            var requiredType = typeof(IMessenger);
            var url = $"tcp://localhost:{serverPort}/{serverName}";
            remoteObject = (IMessenger)Activator.GetObject(requiredType, url);
        }

        public static void StopServer()
        {
            threadRunning = false;
        }

        public static bool Status()
        {
            return threadRunning;
        }

        public static void SendMessage(MsgObject message)
        {
            if(message != null)
            {
                var bytes = MessagePackSerializer.Serialize(message);
                remoteObject.SendMessage(sendId, bytes); 
            }
        }

        private static void RefreshMessages()
        {
            try
            {
                var requiredType = typeof(IMessenger);
                var url = $"tcp://localhost:{serverPort}/{serverName}";
                remoteObject = (IMessenger)Activator.GetObject(requiredType, url);

                threadRunning = true;
                remoteObject.ClearMessage(listenId);

                Console.WriteLine("[MakerBridge] Starting client");
            }
            catch(Exception)
            {
                threadRunning = false;
                Console.WriteLine("[MakerBridge] Server not found");
            }

            while(threadRunning)
            {
                try
                {
                    var msg = remoteObject.GetMessage(listenId);
                    if(msg != null)
                    {
                        var message = MessagePackSerializer.Deserialize<MsgObject>(msg);
                        messageAction(message);
                    }

                    Thread.Sleep(500);
                }
                catch(ArgumentException ex)
                {
                    Console.WriteLine("ERROR: Old bug in MessagePack-CSharp (Duplicate type name within an assembly, issue #127)\n" +
                                      "Must use a fixed Assembly-CSharp-firstpass.dll for this to work with scriptloader in KK\n" + ex);
                    threadRunning = false;
                }
                catch(Exception)
                {
                    threadRunning = false;
                }
            }
            
            Console.WriteLine("[MakerBridge] Server connection lost, starting own server");

            try
            {
                RPCServer.Start(MakerBridge.ServerName, MakerBridge.ServerPort);
                Listen();
            }
            catch(Exception ex)
            {
                Log(LogLevel.Error, ex);
            }
        }
    }
}

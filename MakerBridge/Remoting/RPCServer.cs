using System;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Collections;
using System.Runtime.Serialization.Formatters;

namespace MakerBridge.Remoting
{
    class RPCServer
    {
        static TcpChannel tcpChan;

        public static void Start(string name, int port)
        {
            try
            {
                var serverProv = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
                //RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

                var properties = new Hashtable();
                bool isSecure = false;
                properties["port"] = port;
                properties["typeFilterLevel"] = TypeFilterLevel.Full;
                properties["name"] = name;  // unique channel name

                if(isSecure)  // remoting comm security and encryption
                {
                    properties["secure"] = isSecure;
                    properties["impersonate"] = false;
                }

                tcpChan = new TcpChannel(properties, null, serverProv);
                ChannelServices.RegisterChannel(tcpChan, isSecure);

                Type commonInterfaceType = typeof(RemotingMessenger);
                RemotingConfiguration.RegisterWellKnownServiceType(commonInterfaceType, name, WellKnownObjectMode.Singleton);

                Console.WriteLine("[MakerBridge] Starting server");
            }
            catch(Exception)
            {
                Console.WriteLine("[MakerBridge] Server already started");
            }
        }

        public static void Stop()
        {
            ChannelServices.UnregisterChannel(tcpChan);
        }
    }
}

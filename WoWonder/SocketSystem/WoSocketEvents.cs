using System;
using SocketIOClient;

namespace WoWonder.SocketSystem
{
    public class WoSocketEvents
    {
        public void InitEvents(SocketIO client)
        {
            try
            {
                //All events of sockets connestcion are here
                client.OnDisconnected += Socket_OnDisconnected; 
                client.OnError += Socket_OnError; 
                client.OnReconnectFailed += Socket_OnReconnectFailed;
                client.OnReconnecting += Socket_OnReconnecting;
                client.OnPong += Socket_OnPong;
                client.OnPing += Client_OnPing;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

        private void Client_OnPing(object sender, EventArgs e)
        {
            Console.WriteLine("Socket_OnPong + {0}", e);
        }
        private static void Socket_OnPong(object sender, TimeSpan e)
        {
            Console.WriteLine("Socket_OnPong + {0}", e);
        }

        private static void Socket_OnReconnecting(object sender, int e)
        {
            Console.WriteLine("Socket_OnReconnecting + {0}", e);
        }

        private static void Socket_OnReconnectFailed(object sender, Exception e)
        {
            Console.WriteLine("Socket_OnReconnectFailed + {0}", e);
        }

        private static void Socket_OnError(object sender, string e)
        {
            Console.WriteLine("Socket_OnError + {0}", e);
        }

        private static void Socket_OnDisconnected(object sender, string e)
        {
            Console.WriteLine("Socket_OnDisconnected + {0}", e);
        }
    }
}
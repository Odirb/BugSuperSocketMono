using System;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.WebSocket;

namespace BugSuperSocketMono
{
    class WebSocketServer
    {
        private readonly bool _useTask;

        public WebSocketServer(int port, string pfxPath, string password, bool useTask)
        {
            _useTask = useTask;
            var config = new ServerConfig
            {
                Ip = "127.0.0.1",
                Port = port,
                MaxRequestLength = 64 * 1024 * 1024,
                Security = "tls, tls11, tls12",
                Certificate = new CertificateConfig
                {
                    FilePath = pfxPath,
                    Password = password
                }
            };

            var _server = new SuperSocket.WebSocket.WebSocketServer();
            if (!_server.Setup(config, logFactory: new ConsoleLogFactory()))
                throw new Exception("");

            _server.NewSessionConnected += NewSessionConnected;
            _server.NewMessageReceived += NewMessageReceived;
            _server.SessionClosed += SessionClosed;
            _server.Start();
        }

        private void NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine($"{session.SessionID} : Session is connected.");
        }

        private void NewMessageReceived(WebSocketSession session, string value)
        {
            void ProcessMessageReceived()
            {
                Console.WriteLine($"{session.SessionID} : Message received, path = {session.Path}, value = \"{value}\".");
                if (session.Path == "/ping")
                {
                    session.Send("Pong!");
                    Console.WriteLine($"{session.SessionID} : Server send pong.");
                }
            }

            if (_useTask)
            {
                Console.WriteLine($"{session.SessionID} : Sending response on different thread...");
                Task.Run(ProcessMessageReceived);
            }
            else
            {
                Console.WriteLine($"{session.SessionID} : Sending response on same thread...");
                ProcessMessageReceived();
            }

            Console.WriteLine($"{session.SessionID} : Response sent.");
        }

        private void SessionClosed(WebSocketSession session, CloseReason value)
        {
            Console.WriteLine($"{session.SessionID} : Session is closed.");
        }
    }
}

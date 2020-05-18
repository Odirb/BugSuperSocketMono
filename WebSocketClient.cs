using System;
using System.Threading.Tasks;
using WebSocket4Net;

namespace BugSuperSocketMono
{
    class WebSocketClient
    {
        private readonly WebSocket _webSocket;
        private readonly TaskCompletionSource<bool> _tcs;

        public Task CallCompleted => _tcs.Task;

        public WebSocketClient(int port)
        {
            _tcs = new TaskCompletionSource<bool>();
            _webSocket = new WebSocket($"wss://localhost:{port}/ping");

            _webSocket.Opened += Opened;
            _webSocket.MessageReceived += MessageReceived;
            _webSocket.Closed += Closed;

            _webSocket.Open();
        }

        private void Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Client websocket opened.");
            _webSocket.Send("Hello server!");
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"Client receive message: \"{e.Message}\".");
            _tcs.SetResult(true);
        }

        private void Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Client websocket closed.");
        }
    }
}

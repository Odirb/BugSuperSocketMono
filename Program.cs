using System.Net;
using System.Threading.Tasks;

namespace BugSuperSocketMono
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var useTask = true;
            var port = 14010;

            // Set first command line parameter to "false" to disable thread in WebSocket server event
            if (args.Length == 1)
            {
                bool.TryParse(args[0], out useTask);
            }

            // Disable certificate check to be able to use self signed certificate.
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var server = new WebSocketServer(port, "certificate.pfx", "42", useTask);
            var client = new WebSocketClient(port);

            await client.CallCompleted;
        }
    }
}

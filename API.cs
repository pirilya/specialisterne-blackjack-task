using System;
using System.Net;
using System.Threading.Tasks;

namespace Cardgame.Blackjack {
    public class APIServer {
        public int Port;
        HttpListener Listener;
        public APIServer (int port) {
            Port = port;
            Listener = new HttpListener();
            Listener.Prefixes.Add(String.Format("http://localhost:{0}/", Port));
            Listener.Start();
            Console.WriteLine("Listening for connections on port {0}", Port);
        }
        public async Task Listen() {
            while (true) {
                var context = await Listener.GetContextAsync();
                Console.WriteLine("request! at url {0}", context.Request.Url);
                context.Response.OutputStream.Write(new byte[] {}, 0, 0);
                context.Response.OutputStream.Close();
            }
        }
    }
}
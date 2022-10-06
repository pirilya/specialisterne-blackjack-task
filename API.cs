using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;

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
        void DebugPrint(HttpListenerRequest request) {
            Console.WriteLine("request! at url path {0}, with query {1}, headers:\n{2}", 
            request.Url.AbsolutePath, request.Url.Query, request.Headers);
            string text;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)) {
                text = reader.ReadToEnd();
            }
            Console.WriteLine("the request body is ", text);
        }
        public async Task Listen() {
            while (true) {
                var context = await Listener.GetContextAsync();
                DebugPrint(context.Request);
                context.Response.OutputStream.Write(new byte[] {}, 0, 0);
                context.Response.OutputStream.Close();
            }
        }
    }
}
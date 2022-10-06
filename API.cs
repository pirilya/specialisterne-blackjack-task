using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace Cardgame.Blackjack {
    public class APIServer {
        public int Port;
        public World World;
        HttpListener Listener;
        public APIServer (int port) {
            Port = port;
            World = new World();
            World.AddGame("test-game");
            World.AddPlayer("test-player", 10000.0);
            Listener = new HttpListener();
            Listener.Prefixes.Add(String.Format("http://localhost:{0}/api/", Port));
        }
        byte[] GetData<T> (string id, Func<string,T> Getter) {
            T result;
            try {
                result = Getter(id);
            } catch {
                return new byte[] {};
            }
            return JsonSerializer.SerializeToUtf8Bytes(result);
        }
        void DebugPrint(HttpListenerRequest request) { // delete this function before publishing
            Console.WriteLine("request! at url path {0}, with query {1}, headers:\n{2}", 
            request.Url.AbsolutePath, request.Url.Query, request.Headers);
            string text;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)) {
                text = reader.ReadToEnd();
            }
            Console.WriteLine("the request body is ", text);
        }
        public async Task Listen() {
            Console.WriteLine("Listening for connections on port {0}", Port);
            Listener.Start();
            while (true) {
                var context = await Listener.GetContextAsync();
                var urlParams = context.Request.Url.Query.Substring(1).Split("&")
                                    .Select(x => x.Split("=", 2))
                                    .ToDictionary(keySelector: x => x[0], elementSelector: x => x[1]);
                if (context.Request.Url.AbsolutePath == "/api/getdata") {
                    var jsonData = new byte[] {};
                    if (urlParams["type"] == "player") {
                        jsonData = GetData(urlParams["id"], World.GetPlayer);
                    } else if (urlParams["type"] == "game") {
                        jsonData = GetData(urlParams["id"], World.GetGame);
                    }
                    context.Response.ContentType = "text/json";
                    context.Response.OutputStream.Write(jsonData, 0, jsonData.Length);
                    context.Response.OutputStream.Close();
                }
                else {
                    DebugPrint(context.Request);
                    context.Response.OutputStream.Write(new byte[] {}, 0, 0);
                    context.Response.OutputStream.Close();
                }
            }
        }
    }
}
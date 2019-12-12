using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();

            var baseUrl = "https://localhost:5001";

            var connection = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/waithookhub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<string>("Hook", s =>
            {
                var msg = JsonSerializer.Deserialize<WaithookMessage>(s);
                Console.WriteLine(msg.Body);
            });

            await connection.StartAsync();

            while (connection.State != HubConnectionState.Connected)
                await Task.Delay(TimeSpan.FromSeconds(1));

            var client = new HttpClient();
            var model = new
            {
                id = "some id",
                name = "jarle"
            };
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/waithook/{connection.ConnectionId}", content);
            Console.WriteLine(response.StatusCode);
            Console.ReadLine();

        }
    }

    public class WaithookMessage
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
    }
}

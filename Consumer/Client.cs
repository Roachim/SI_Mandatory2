using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;




namespace Consumer
{
    

    public class Client
    {
        private static string _path = "lastReceived/";




        #region Methods

        public static async void RequestMessage()
        {
            System.Console.WriteLine("Enter topic:");
            string topic = Console.ReadLine();

            System.Console.WriteLine("Enter format:");

            string format = Console.ReadLine().ToLower();

            string filePath = $"{_path}{topic}.txt";

            string lastreceived = File.Exists(filePath) ? ReadAndUpdateLastReceived(filePath) : CreateAndReadLastReceived(filePath);

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync($"http://localhost:5000/message/retrieve?topic={topic}&format={format}&lastReceived={lastreceived}");

            if (!response.IsSuccessStatusCode)
            {
                UndoLastReceivedUpdate(filePath, lastreceived);
                Console.WriteLine($"Statuscode: {response.StatusCode}");
                return;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            
            List<string> messages = JsonSerializer.Deserialize<List<string>>(jsonResponse);

            foreach (string message in messages)
            {
                Console.WriteLine("########################");
                Console.WriteLine(message);
                Console.WriteLine();
            }
            

            Console.WriteLine("Press any key to request new message");
            Console.ReadLine();

        }
             
        #endregion

        #region HelpMethods

        private static string ReadAndUpdateLastReceived(string filePath)
        {
            string lastReceived = File.ReadAllText(filePath);
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            return lastReceived;
        }

        private static string CreateAndReadLastReceived(string filePath)
        {
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            return "0";
        }

        private static void UndoLastReceivedUpdate(string filePath, string oldLastReceived)
        {
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
        }
             
        #endregion
    }
}
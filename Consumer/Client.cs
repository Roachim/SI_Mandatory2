using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;




namespace Consumer
{
    

    public class Client
    {
        private static string _path = "lastReceived/";
        private static string _baseURL = "http://localhost:5000/message/";
        private static HttpClient _client = new HttpClient();


        #region Methods

        public static async Task MakeRequest()
        {
            Console.WriteLine("\nEnter id:");
            string id = Console.ReadLine().ToLower();

            Console.WriteLine("Enter topic:");
            string topic = Console.ReadLine().ToLower();

            Console.WriteLine("Enter format:");
            string format = Console.ReadLine().ToLower();

            Console.WriteLine("Choose Action:\n\t1. All\n\t2. Unreceived");
            string action = Console.ReadLine().ToLower();

            string directories = $"{_path}{topic}/";

            string filePath = $"{directories}{id}.txt";

            CreateDirectories(directories);

            switch (action)
            {
                case "all":
                    await RequestAll(filePath, topic, format);
                    break;
                case "unreceived":
                    await RequestMessages(filePath, topic, format);
                    break;
                default:
                    Console.WriteLine("Incorrect Action, Please write either: All or Unreceived");
                    break;
            }
        }

        public static async Task RequestAll(string filePath, string topic, string format)
        {

            HttpResponseMessage response = await _client.GetAsync($"{_baseURL}retrieveAll?topic={topic}&format={format}");

            if (response.IsSuccessStatusCode)
            {
                ReadAndUpdateLastReceived(filePath);

                DisplayMessages(response);

            }
            else
            {
                Console.WriteLine($"Statuscode: {response.StatusCode}");
            }
        }

        public static async Task RequestMessages(string filePath, string topic, string format)
        {
            

            string lastreceived = File.Exists(filePath) ? ReadAndUpdateLastReceived(filePath) : CreateLastReceived(filePath);

            HttpResponseMessage response = await _client.GetAsync($"{_baseURL}retrieve?topic={topic}&format={format}&lastReceived={lastreceived}");

            if (!response.IsSuccessStatusCode)
            {
                UndoLastReceivedUpdate(filePath, lastreceived);
                Console.WriteLine($"Statuscode: {response.StatusCode}");
                return;
            }

            DisplayMessages(response);
            

            Console.WriteLine("Press any key to request new message");
            Console.ReadLine();

        }
             
        #endregion

        #region HelpMethods

        private static void CreateDirectories(string directories)
        {
            if (!Directory.Exists(directories))
            {
                Directory.CreateDirectory(directories);
            }
        }

        private static string ReadAndUpdateLastReceived(string filePath)
        {
            string lastReceived = File.ReadAllText(filePath);
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            return lastReceived;
        }

        private static string CreateLastReceived(string filePath)
        {
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            return "0";
        }

        private static void UndoLastReceivedUpdate(string filePath, string oldLastReceived)
        {
            File.WriteAllText(filePath, oldLastReceived);
        }

        private static async void DisplayMessages(HttpResponseMessage response)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            
            List<string> messages = JsonSerializer.Deserialize<List<string>>(jsonResponse);

            foreach (string message in messages)
            {
                Console.WriteLine("########################");
                Console.WriteLine(message);
                Console.WriteLine();
            }
        }
             
        #endregion
    }
}
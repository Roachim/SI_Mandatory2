using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;




namespace Consumer
{
    
    /// <summary>
    /// Provides a method for requesting messages from the messagequeue
    /// </summary>
    public class Client
    {
        private static string _path = "lastReceived/";
        private static string _baseURL = "http://localhost:5000/message/";
        private static HttpClient _client = new HttpClient();


        #region Methods

        /// <summary>
        /// Let's the user input id, topic, format and an action.
        /// The action determines wheter to call RequestAllAsync() or RequestMessagesAsync()
        /// If the folder lastReceived does not exist then it is created
        /// If the chosen topic does not exist then a folder with its name will be created in lastReceived
        /// </summary>
        /// <returns></returns>
        public static async Task MakeRequestAsync()
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

            CreateDirectories(directories);//Creates all directories in the path if they do not exist

            switch (action)
            {
                case "all":
                    await RequestAllAsync(filePath, topic, format);
                    break;
                case "unreceived":
                    await RequestMessagesAsync(filePath, topic, format);
                    break;
                default:
                    Console.WriteLine("Incorrect Action, Please write either: All or Unreceived");
                    break;
            }
        }

        /// <summary>
        /// Retrieves all messages of a topic from the messagesqueue in the desired format and updates the lastReceived file accordingly
        /// </summary>
        /// <param name="filePath">The relative path(inluding the file's name and type) to the file. E.g. lastreceived/Numbers/1.txt</param>
        /// <param name="topic">Name of the topic to retrieve from the messagequeue</param>
        /// <param name="format">The format that the messages should be returned as</param>
        /// <returns></returns>
        private static async Task RequestAllAsync(string filePath, string topic, string format)
        {

            HttpResponseMessage response = await _client.GetAsync($"{_baseURL}retrieveAll?topic={topic}&format={format}");

            if (response.IsSuccessStatusCode)
            {
                if (File.Exists(filePath))
                {
                    ReadAndUpdateLastReceived(filePath);
                }
                else
                {
                    CreateLastReceived(filePath);
                }
                
                await DisplayMessagesAsync(response);

            }
            else
            {
                Console.WriteLine($"Statuscode: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Retrieves all unreceived messages of a topic from the messagesqueue in the desired format and updates the lastReceived file accordingly
        /// </summary>
        /// <param name="filePath">The relative path(inluding the file's name and type) to the file. E.g. lastreceived/Numbers/1.txt</param>
        /// <param name="topic">Name of the topic to retrieve from the messagequeue</param>
        /// <param name="format">The format that the messages should be returned as</param>
        /// <returns></returns>
        private static async Task RequestMessagesAsync(string filePath, string topic, string format)
        {
            

            string lastreceived = File.Exists(filePath) ? ReadAndUpdateLastReceived(filePath) : CreateLastReceived(filePath);

            HttpResponseMessage response = await _client.GetAsync($"{_baseURL}retrieve?topic={topic}&format={format}&lastReceived={lastreceived}");

            if (!response.IsSuccessStatusCode)
            {
                UndoLastReceivedUpdate(filePath, lastreceived);//Undoes the changes to the lastReceived file in case of error
                Console.WriteLine($"Statuscode: {response.StatusCode}");
                return;
            }

            await DisplayMessagesAsync(response);
        }
             
        #endregion

        #region HelpMethods

        /// <summary>
        /// Creates all directories and subdirectories that are part of the string, unless they already exist
        /// </summary>
        /// <param name="directories">The path for all directories that should be created</param>
        private static void CreateDirectories(string directories)
        {
            if (!Directory.Exists(directories))
            {
                Directory.CreateDirectory(directories);
            }
        }

        /// <summary>
        /// Reads the current value from the specified lastreceived file 
        /// and then updates the file with the value for the epoch in melliseconds for the current time
        /// </summary>
        /// <param name="filePath">The relative path(inluding the file's name and type) to the file. E.g. lastreceived/Numbers/1.txt</param>
        /// <returns>The read string value from the lastReceived file</returns>
        private static string ReadAndUpdateLastReceived(string filePath)
        {
            string lastReceived = File.ReadAllText(filePath);
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            return lastReceived;
        }

        /// <summary>
        /// Creates a lastReceived file with the value for the epoch in melliseconds for the current time
        /// This method is called when the user has not requested a topic before.
        /// </summary>
        /// <param name="filePath">The relative path(inluding the file's name and type) to the file. E.g. lastreceived/Numbers/1.txt</param>
        /// <returns>0</returns>
        private static string CreateLastReceived(string filePath)
        {
            File.WriteAllText(filePath, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            return "0";
        }

        /// <summary>
        /// Reverts the lastReceived file's value to the one it had before the request was made.
        /// </summary>
        /// <param name="filePath">The relative path(inluding the file's name and type) to the file. E.g. lastreceived/Numbers/1.txt</param>
        /// <param name="oldLastReceived">The lastReceived file's value before the request was made</param>
        private static void UndoLastReceivedUpdate(string filePath, string oldLastReceived)
        {
            File.WriteAllText(filePath, oldLastReceived);
        }

        /// <summary>
        /// Displays the received messages in a console window
        /// </summary>
        /// <param name="response">The http response received from the messagequeue</param>
        /// <returns>void</returns>
        private static async Task DisplayMessagesAsync(HttpResponseMessage response)
        {
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
    }
}
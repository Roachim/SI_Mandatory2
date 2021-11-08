using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;




namespace Consumer
{
    

    public class Client
    {
        private static string _path = "lastReceived/";




        #region Methods

        public static void RequestMessage()
        {
            System.Console.WriteLine("Enter topic:");
            string topic = Console.ReadLine();

            System.Console.WriteLine("Enter format:");

            string format = Console.ReadLine();

            string filePath = $"{_path}{topic}.txt";

            string lastreceived = File.Exists(filePath) ? ReadAndUpdateLastReceived(filePath) : CreateAndReadLastReceived(filePath);

            HttpClient client = new HttpClient();

            Task<HttpResponseMessage> response = client.GetAsync($"http://localhost:5000/message/retrieve?topic={topic}&format={format}&lastReceived={lastreceived}");

            if (!response.Result.IsSuccessStatusCode)
            {
                UndoLastReceivedUpdate(filePath, lastreceived);
                Console.WriteLine($"Statuscode: {response.Result.StatusCode}");
                return;
            }

            Console.WriteLine(response.Result.Content.ReadAsStringAsync());

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
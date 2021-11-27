using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MessageQueue.DTO;
using MessageQueue.DTOinterfaces;
using System.Collections;
using System.Net.Http;
using System.Text;



namespace MessageQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        //path to folder for all messages
        string messageFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Messages");
        //make httpClient to handle http requests
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// A function for consumers to get all files they have yet to receive
        /// </summary>
        /// <param name="topic">The folder for the files they wish to receive</param>
        /// <param name="format">The format they wish to receive the file text in</param>
        /// <param name="lastreceived">The timestamp the user gives to indicate when they last received files, down to miliseconds</param>
        /// <returns>A converted string with all the content from files</returns>
        [HttpGet]
        [Route("retrieve")]
        public async Task<string> RetrieveMessage([FromQuery]string topic, [FromQuery]String format, [FromQuery]string lastreceived)
        {
            //case insensitize
            topic = topic.ToLower();
            format = format.ToLower();

            //Path indicated by topic
            string folderPath = System.IO.Path.Combine(messageFolder, topic);

            if (!System.IO.Directory.Exists(folderPath))
            {
                return "Topic does not exist";
            }
            
            
            //Have a list to add every object made from files in folderpath
            List<Message> messageList = new List<Message>();

            DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach(var file in di.GetFiles()){
                //Below function gets the epoch by removing the first random letters and removing file --
                //format from name by splitting after any '.' and taking everything before it.
                //epoch indcates timeframe of what files in folder to send to getter
                System.Console.WriteLine("################");
                System.Console.WriteLine(file.Name);
                string epoch = file.Name.Substring(4);
                epoch = epoch.Split('.',2)[0];
                string fileFormat = file.Name.Split('.',2)[1];
                
                //- find every instance meeting criteria
                //- Any instance will become 'message' object with their format
                //- They will then be added to 'MessageList' object
                //- 'MessageList' will also get a string for the format that every 'Message' is to be converted to.
                if(Convert.ToInt64(lastreceived)  < Convert.ToInt64(epoch)){
                    Message message = new Message(file.OpenText().ReadToEnd(), fileFormat);
                    messageList.Add(message);
                }
            }
            //Send this to python module
            MessageList packet = new MessageList(messageList, format);
            //make http content
            HttpContent content = new StringContent(JsonSerializer.Serialize(packet), Encoding.UTF8, "application/json");
            //Make http request to python module
            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:6000/convert", content);
            
            //return all files to consumer
            return await response.Content.ReadAsStringAsync();
        }
        
        /// <summary>
        /// A function for consumers to get all files
        /// </summary>
        /// <param name="topic">The folder for the files they wish to receive</param>
        /// <param name="format">The format they wish to receive the file text in</param>
        /// <returns>A converted string with all the content from files</returns>
        [HttpGet]
        [Route("retrieveAll")]
        public async Task<string> RetrieveAll([FromQuery]string topic, [FromQuery]String format)
        {
            //case insensitize
            topic = topic.ToLower();
            format = format.ToLower();

            //Path indicated by topic
            string folderPath = System.IO.Path.Combine(messageFolder, topic);

            if (!System.IO.Directory.Exists(folderPath))
            {
                return "Topic does not exist";
            }
            // -- maybe make a catch for when user tries to use none-supported format?

            //Have a list to add every object made from files in folderpath
            List<Message> messageList = new List<Message>();

            DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach(var file in di.GetFiles()){
                //Get format by splitting by '.' and getting the latter part.
                // System.Console.WriteLine("################");
                // System.Console.WriteLine(file.Name);
                string fileFormat = file.Name.Split('.',2)[1];
                
                //- Every instance will become a 'message' object with their format attached
                //- They will then be added to 'MessageList' object
                //- 'MessageList' will also get a string for the format that every 'Message' is to be converted to.
                
                Message message = new Message(file.OpenText().ReadToEnd(), fileFormat);
                messageList.Add(message);
            }
            //make packet to send to python converter
            MessageList packet = new MessageList(messageList, format);
            //Make content for http request
            HttpContent content = new StringContent(JsonSerializer.Serialize(packet), Encoding.UTF8, "application/json");
            //Make http request to python converter
            HttpResponseMessage response = await client.PostAsync("http://127.0.0.1:6000/convert", content);
            
            //return all files
            return await response.Content.ReadAsStringAsync();
        }


        /// <summary>
        /// Creates a file from any text message sent from any person.
        /// File is saved in as its own text file in the designated folder
        /// </summary>
        /// <param name="fileString">String sent us, may be JSON, CSV, XML or TSV (maybe YAML?)</param>
        /// <param name="topic">Topic designates the folder this file will be saved in. If folder for topic does not exist: will be created</param>
        /// <param name="format">The format that the string was sent as. Will be saved as this type. Wait, what if the wrong format is given?!</param>
        /// <returns>A String. A single message confirming the state of the file, or whether or not there was an error</returns>
        [HttpPost]
        [Route("create")]
        public string CreateMessage([FromBody]CreateMessageDTO File)
        {
            //Folder for messages
            System.IO.Directory.CreateDirectory(messageFolder); //check if (exist): if not (create)
            //Folder for messages for that topic
            string folderPath = System.IO.Path.Combine(messageFolder, File.Topic);

            //Check if folder exist, if not: create it
            System.IO.Directory.CreateDirectory(folderPath); //this method automatically checks if folder exist before creating it.

            //Write name for file (4 random numbers + date + time + miliseconds)
            var rand = new Random();
            string numbers = "";
            //assign 4 random numbers in beginning of file
            for(int i = 0; i<=3; i++)
            {
                numbers = numbers +rand.Next(10);
            }
            string fileName = numbers + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()+"."+File.Format;

            //Write file with content in given path. folderpath includes name for new file.
            folderPath = System.IO.Path.Combine(folderPath, fileName);
            if(!System.IO.File.Exists(folderPath))
            {
                try{
                    System.IO.File.WriteAllText(folderPath, File.FileString); //won't accept just "File" as starter for method, must include "System.IO" first for some reason
                }
                catch
                {
                    return "File Error";
                }
                
            }
            else
            {
                return "File aready exist";
            }
            
            //done

            return "File Recorded in queue";
        }
        

    }

    
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json.Serialization;


namespace MessageQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        //path to folder for all messages
        string messageFolder = System.IO.Path.Combine(System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName, "Messages");
        
        //for consumer to recieve text from a file
        //Using the topic, format and lastreceived
        //search folders for given topic
        //send all messages where Epoch > lastreceived 
        // - (last received is date and time down to milliseconds - convert to epoch for comparison)
        //All valid messages gets converted to the given format
        //converted messages are then send back to the consumer
        [HttpGet]
        [Route("retrieve")]
        public string RetrieveMessage([FromQuery]string topic, [FromQuery]String format, [FromQuery]string lastreceived)
        {
            //Path indicated by topic
            string folderPath = System.IO.Path.Combine(messageFolder, topic);
            try{
                System.IO.Directory.Exists(folderPath);
            }
            catch
            {
                return "Folder for that topic does not exist";
            }
            //epoch indcates timeframe of what files in folder to send to getter
            
            DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach(var file in di.GetFiles()){
                //below should also exlude anything after a '.'
                string epoch = "" + file.Name.Substring(3);
                
                if(Convert.ToInt32(lastreceived)  < Convert.ToInt32(epoch)){
                    //convert to format
                    //add new formatted string to final file
                }
            }

            //format indicates what format files should be converted to before sending

            //have methods to convert from - to different formats here
            //switch;

            //return all files
            return null;
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
        public string CreateMessage([FromBody]string fileString, [FromBody]string topic, [FromBody]string format)
        {
            //Folder for messages
            System.IO.Directory.CreateDirectory(messageFolder); //check if (exist): if not (create)
            //Folder for messages for that topic
            string folderPath = System.IO.Path.Combine(messageFolder, topic);

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
            string fileName = numbers + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()+"."+format;

            //Write file with content in given path. folderpath includes name for new file.
            folderPath = System.IO.Path.Combine(folderPath, fileName);
            if(!System.IO.File.Exists(folderPath))
            {
                try{
                    System.IO.File.WriteAllText(folderPath, fileString); //won't accept just "File" as starter for method, must include "System.IO" first for some reason
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

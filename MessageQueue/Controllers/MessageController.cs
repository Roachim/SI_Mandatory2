using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;


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
        //send all messages that Epoch > then lastreceived 
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
        //Receive a post from any source
        //Save a posted file in folder with same name as topic topic if it exist - otherwise
        // - Create a folder for named topic if it does not exist - then save file to it
        //Topic name should be case insensitive (make function for this)
        //file name should be 4 random numbers + date and time down to milisecond (have epoch converter)
        [HttpPost]
        [Route("create")]
        public string CreateMessage([FromBody]string fileString, [FromBody]string topic, [FromBody]string format)
        {
            //Folder for messages
            System.IO.Directory.CreateDirectory(messageFolder); //check if (exist): if not (create)

            //TopicFolder ------ Get the topic from the file read from the post message!!!!!------------------------

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

            //Write file with content in given path. path includes name for new file
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

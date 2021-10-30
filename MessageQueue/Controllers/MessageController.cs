using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MessageQueue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        
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
            
        }
        //Receive a post from any source
        //Save a posted file in folder with same name as topic topic if it exist - otherwise
        // - Create a folder for named topic if it does not exist - then save file to it
        //Topic name should be case insensitive (make function for this)
        //file name should be 4 random numbers + date and time down to milisecond (have epoch converter)
        [HttpPost]
        [Route("create")]
        public string CreateMessage(string file)
        {
            //Masterfolder
            string altFolderName = @"C:\Users\joach\Documents\KEA\Semester 2\System Integration\SI_Mandatory2\MessageQueue\Messages";
            //Folder for messages
            string  folderName = 
            System.IO.Path.Combine(System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName, "Messages");

            //TopicFolder ------ Get the topic from the file read from the post message!!!!!------------------------
            string topic = "";
            folderName = System.IO.Path.Combine(folderName, topic);

            //Check if folder exist, if not: create it
            if(!System.IO.Directory.Exists(folderName)){
                System.IO.Directory.CreateDirectory(folderName);
            }
            
            

        }
    }
}

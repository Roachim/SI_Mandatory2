using MessageQueue.DTOinterfaces;

namespace MessageQueue.DTO
{
    public class CreateMessageDTO : ICreateMessageDTO
    {
        public string FileString { get; set; }
        public string Topic { get; set; }
        public string Format { get; set; }
        public CreateMessageDTO(string fileString, string topic, string format){
            FileString=fileString;
            Topic=topic;
            Format=format;
        }
        
    }
}
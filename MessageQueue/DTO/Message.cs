using MessageQueue.DTOinterfaces;

namespace MessageQueue.DTO
{
    public class Message : Imessage
    {
        public string Text { get; set; }
        public string Format { get; set; }
        public Message(string text, string format){
            Text = text;
            Format = format;
        }
    }
}
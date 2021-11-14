using MessageQueue.DTOinterfaces;
using System.Collections.Generic;

namespace MessageQueue.DTO
{
    public class MessageList : ImessageList
    {
        public IEnumerable<Imessage> Messages { get; set; }
        public string FormatTo { get; set; }

        public MessageList(IEnumerable<Message> messages, string formatTo){
            Messages = messages;
            FormatTo = formatTo;
        }
    }
}
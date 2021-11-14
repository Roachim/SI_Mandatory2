using System;
using System.Collections;

namespace MessageQueue.DTOinterfaces
{
    public interface ICreateMessageDTO
    {
         string FileString { get; set; }
         string Topic { get; set; }
         string Format { get; set; }
    }
}
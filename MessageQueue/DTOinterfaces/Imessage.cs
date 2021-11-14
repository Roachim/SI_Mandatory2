using System;
using System.Collections;

namespace MessageQueue.DTOinterfaces
{
    public interface Imessage
    {
         string Text { get; set; }
         string Format { get; set; }
    }
}
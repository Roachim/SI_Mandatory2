using System;
using System.Collections;
using System.Collections.Generic;
using MessageQueue.DTOinterfaces;

namespace MessageQueue.DTOinterfaces
{
    public interface ImessageList
    {
         IEnumerable<Imessage> Messages { get; set; }
         string FormatTo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class MessageModel
    {
        public string Body { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Destinations { get; set; }
        public MessageType MessageType { get; set; }
    }

    [Flags]
    public enum MessageType 
    {
        Push,
        Email,
        SMS
    }
}

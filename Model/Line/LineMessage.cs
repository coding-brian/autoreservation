using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Line
{
    public class LineMessage
    {
        public String destination { get; set; }

        public List<Event> events { get; set; }

    }

    public class Event 
    { 
        public String replyToken { get; set; }

        public String type { get; set; }

        public String mode { get; set; }

        public long timestamp { get; set; }

        public Source source { get; set; }

        public Message message { get; set; }
    }

    public class Source
    {
        public String type { get; set; }

        public String userId { get; set; }

    }

    public class Message
    { 
        public String id { get; set; }
        public String type { get; set; }
        public String text { get; set; }
    }
}

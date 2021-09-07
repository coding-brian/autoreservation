using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Line
{
    public class ReplyMessageRequest
    {
        public string replyToken { get; set; }

        public List<ReplyMessages> messages { get; set; } = new List<ReplyMessages>();
    }

    public class ReplyMessages 
    {
        public string type { get; set; }

        public string text { get; set; }
    }
}

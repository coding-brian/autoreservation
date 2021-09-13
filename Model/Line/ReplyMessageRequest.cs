using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Line
{
    public class ReplyMessageRequest
    {
        public ReplyMessageRequest()
        {
            this.messages = new List<object>();
        }

        public string replyToken { get; set; }

        public List<Object> messages { get; set; } 
    }
}

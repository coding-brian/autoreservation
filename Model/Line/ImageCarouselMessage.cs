using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Line
{
    public class ImageCarouselMessage
    {
        public ImageCarouselMessage()
        {
            template = new Template();
        }
        public string type { get; } = "template";

        public string altText { get; set; }

        public Template template { get; set; }
    }

    public class Template
    {
        public string type { get; } = "image_carousel";

        public List<Colums> columns { get; set; } = new List<Colums>();
    }

    public class Colums
    {
        public string imageUrl { get; set; }

        public Object action { get; set; }
    }

    public class MessageAction
    {
        public string type { get; set; } = "message";

        public string label { get; set; }
        public string text { get; set; }
    }

    public class PostBackAction
    {
        public string type { get; set; } = "postback";

        public string label { get; set; }
        public string data { get; set; }
        public string text { get; set; }
    }
}

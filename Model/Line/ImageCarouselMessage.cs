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
        public string type { get;} = "template";

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

        public Action action { get; set; } = new Action();
    }

    public class Action 
    {
        public string type { get; set; } = "message";

        public string label { get; set; }
        public string text { get; set; }
    }
}

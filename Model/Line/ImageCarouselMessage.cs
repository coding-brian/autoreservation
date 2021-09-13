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
        public string Type { get; } = "image_carousel";

        public List<Colums> Columns { get; set; } = new List<Colums>();
    }

    public class Colums 
    {
        public string imageUrl { get; set; }

        public List<Action> action { get; set; } = new List<Action>();
    }

    public class Action 
    {
        public string Type { get; } = "message";

        public string Label { get; set; }
        public string Text { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Line
{
    public class ImageCarouselMessage
    {
        public string type { get;} = "template";

        public string AltText { get; set; }

        public Template template { get; set; }
    }

    public class Template 
    {
        public string Type { get; } = "image_carousel";

        public List<Colums> Columns { get; set; }
    }

    public class Colums 
    {
        public string imageUrl { get; set; }

        public List<Action> action { get; set; }
    }

    public class Action 
    {
        public string Type { get; } = "message";

        public string Label { get; set; }
        public string Text { get; set; }
    }
}

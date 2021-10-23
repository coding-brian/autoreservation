using Model.Line;
using Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.MessageFatcory
{
    public class MessageFactory : IMessageFactory
    {
        public TextMessage GenerateTextMessageAsyc(string text)
        {
            var message = new TextMessage();

            message.text = text;

            return message;
        }

        public ImageCarouselMessage GenerateImageCarouselMessage(string altText, List<Column> columns)
        {
            var imageCarouselMessage = new ImageCarouselMessage();

            foreach (var column in columns) 
            {
                imageCarouselMessage.template.columns.Add(column);
            }

            imageCarouselMessage.altText = altText;

            return imageCarouselMessage;
        }

        public Object ActinoGenerate(string actiontype, Object dataObject)
        {
            var action = new Object();
            var type = dataObject.GetType();
            try
            {
                switch (actiontype)
                {
                    case "message":
                        action = new MessageAction();
                        break;
                    case "postback":
                        action = new PostBackAction();
                        break;
                }

                Parallel.ForEach(type.GetProperties(), (property) =>
                {
                    var value = property.GetValue(dataObject);
                    Parallel.ForEach((action.GetType().GetProperties()), (actionProperty) =>
                    {
                        if (actionProperty.Name == property.Name)
                        {
                            actionProperty.SetValue(action, value);
                        }
                    });
                });
            }
            catch (Exception e)
            {
                throw;
            }

            return action;
        }
    }
}

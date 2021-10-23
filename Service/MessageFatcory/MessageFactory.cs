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
        //public MessageFactory(ICoachRepository coachRepository)
        //{
        //    _coachRepository = coachRepository;
        //}

        public TextMessage GenerateTextMessageAsyc(string text)
        {
            var message = new TextMessage();

            message.text = text;

            return message;
        }

        public ImageCarouselMessage GenerateImageCarouselMessage(string altText, List<Column> columns)
        {
            var imageCarouselMessage = new ImageCarouselMessage();

            //var coaches = await _coachRepository.SelectCoaches();

            //switch (actionType)
            //{
            //    case "postback":
            //        var postBackmessageObjct = new PostBackAction()
            //        {
            //            type = actionType,
            //            label = label,
            //            text = text,
            //            data = data
            //        };
            //        foreach (var column in columns)
            //        {
            //            column.action = ActinoGenerate(actionType, postBackmessageObjct);
            //            imageCarouselMessage.template.columns.Add(column);
            //        }
            //        break;
            //    case "message":
            //        var messageObjct = new MessageAction()
            //        {
            //            type = actionType,
            //            label = label,
            //            text = text
            //        };
            //        foreach (var column in columns)
            //        {
            //            column.action = ActinoGenerate(actionType, messageObjct);
            //            imageCarouselMessage.template.columns.Add(column);
            //        }
            //        break;
            //}

            foreach (var column in columns) 
            {
                imageCarouselMessage.template.columns.Add(column);
            }

            imageCarouselMessage.altText = altText;

            return imageCarouselMessage;
            //foreach (var coach in coaches)
            //{
            //    var messageObjct = new PostBackAction
            //    {
            //        type = "postback",
            //        label = coach.Name,
            //        text = $"我想查看{coach.Name}可以預約的時間",
            //        data = $"showreservation=true&&coach={coach.id}"
            //    };
            //    column.action = ActinoGenerate(actionType, messageObjct);
            //    imageCarouselMessage.template.columns.Add(column);
            //}

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

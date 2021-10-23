using Model.Line;
using System;
using System.Collections.Generic;

namespace Service.MessageFatcory
{
    public interface IMessageFactory
    {
        ImageCarouselMessage GenerateImageCarouselMessage(string altText, List<Column> columns);
        TextMessage GenerateTextMessageAsyc(string text);

        Object ActinoGenerate(string actiontype, Object dataObject);
    }
}
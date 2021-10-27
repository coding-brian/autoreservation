using Model.Line;
using System.Threading.Tasks;

namespace Service.GenerateMessage
{
    public interface IGenerateMessage
    {
        Task<ImageCarouselMessage> GenerateImageCarourselMessage();
        TextMessage GenerateTextMessage(string text);
    }
}
using Service.GenerateMessage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.WordProcess
{
    public class NoWordProcess : IWordProcess
    {
        private readonly IGenerateMessage _generateMessage;

        public NoWordProcess(IGenerateMessage generateMessage)
        {
            _generateMessage = generateMessage;
        }

        public async Task<object> ProcessWord(string userId)
        {
            var message = await Task.Run(() => { return "你好"; });

            var result= _generateMessage.GenerateTextMessage(message);

            return result;
        }
    }
}

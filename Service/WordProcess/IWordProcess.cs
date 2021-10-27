using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.WordProcess
{
    public interface IWordProcess
    {
        Task<object> ProcessWord(string userId);
    }
}

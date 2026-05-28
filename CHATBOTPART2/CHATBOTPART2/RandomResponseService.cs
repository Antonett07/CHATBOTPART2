using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHATBOTPART2.Services
{
    public class RandomResponseService
    {
        private readonly Random random = new();

        public string GetRandomResponse(List<string> responses)
        {
            return responses[random.Next(responses.Count)];
        }
    }
}

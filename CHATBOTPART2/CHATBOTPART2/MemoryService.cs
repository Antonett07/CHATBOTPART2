using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CHATBOTPART2.Services
{
    public class MemoryService
    {
        public UserMemory Memory { get; } = new();

        public void SaveUserName(string name)
        {
            Memory.UserName = name ?? string.Empty;
        }

        public void SaveFavouriteTopic(string topic)
        {
            Memory.FavouriteTopic = topic ?? string.Empty;
        }

        public void SaveLastTopic(string topic)
        {
            Memory.LastTopic = topic ?? string.Empty;
        }
    }
}

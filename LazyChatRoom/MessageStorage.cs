using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyChatRoom
{
    public interface IMessageStorage
    {
        void Add(string key, string value);

        string Remove(string key);
    }

    public class MessageStorage : IMessageStorage
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            dict.Add(key, value);
        }

        public string Remove(string key)
        {
            var groupName = dict[key];
            dict.Remove(key);
            return groupName;
        }
    }
}

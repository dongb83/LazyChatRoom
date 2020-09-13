using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyChatRoom
{
    public class ChatHub : Hub
    {
        private readonly IMessageStorage _messageStorage;

        public ChatHub(IMessageStorage messageStorage)
        {
            _messageStorage = messageStorage;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                await base.OnConnectedAsync();

                var request = Context.GetHttpContext().Request;
                var chatName = request.Query.Keys.Contains("chatName") ?
                    request.Query["chatName"].ToString() : "chatName";
                _messageStorage.Add(Context.ConnectionId, chatName);
                await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var groupName =_messageStorage.Remove(Context.ConnectionId);
            Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(user);
                var unionid = dict["unionid"].ToString();
                var avatar = dict["headimgurl"].ToString();
                var chatName = dict["chatName"].ToString();

                await Clients.Group(chatName).SendAsync("ReceiveMessage", unionid, avatar, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}

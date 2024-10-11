using Microsoft.AspNetCore.SignalR;
using OMMS.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.SignalR.HubServices
{
    public class ChatHubService
    {
    private readonly IHubContext<ChatHub> _hubContext;

        public ChatHubService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessage(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}

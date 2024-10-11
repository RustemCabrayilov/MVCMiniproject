using Microsoft.AspNetCore.SignalR;
using OMMS.SignalR.Hubs;
using OMMS.SignalR.HubServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.SignalR.HubServices
{
	public class ProductHubService:IProductHubService
	{
		private readonly IHubContext<ProductHub> _hubContext;

		public ProductHubService(IHubContext<ProductHub> hubContext)
		{
			_hubContext = hubContext;
		}
		public async Task ProductAddedMessage(string message)
		{
			await _hubContext.Clients.All.SendAsync("ReceiveMessage",message);
		}
	}
}

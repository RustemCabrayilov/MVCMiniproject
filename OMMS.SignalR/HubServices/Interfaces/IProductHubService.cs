using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.SignalR.HubServices.Interfaces
{
	public interface IProductHubService
	{
		public  Task ProductAddedMessage(string message);
	}
}

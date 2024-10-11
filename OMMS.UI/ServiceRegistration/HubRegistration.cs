using OMMS.SignalR.Hubs;

namespace OMMS.UI.ServiceRegistration
{
	public static class HubRegistration
	{
		public static void MapHubs(this WebApplication app)
		{
			app.MapHub<ChatHub>("/chatHub");
			app.MapHub<ProductHub>("/productHub");
		}
	}
}

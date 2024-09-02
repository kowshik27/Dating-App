using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User == null) 
            throw new HubException("Cannot get Username to the Hub");

        var isOnline = await tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);

        if(isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUserName());

        var currentUsers = await tracker.GetOnlineUsers();

        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.User == null) throw new HubException("Cannot get Username to the Hub");
        
        var isOffline = await tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);

        if(isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUserName());

        await base.OnDisconnectedAsync(exception);
    }

    

    
}

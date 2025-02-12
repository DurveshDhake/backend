using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> UserConnections = new();

    // When a user connects, store their connection ID
    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier ?? Context.ConnectionId;
        UserConnections[userId] = Context.ConnectionId;
        return base.OnConnectedAsync();
    }

    // When a user disconnects, remove their connection
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier ?? Context.ConnectionId;
        UserConnections.TryRemove(userId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    // Send message from a customer to the owner
    public async Task SendMessageToOwner(string customerId, string message)
    {
        if (UserConnections.TryGetValue("Owner", out var ownerConnectionId))
        {
            await Clients.Client(ownerConnectionId).SendAsync("ReceiveMessage", customerId, message);
        }
    }

    // Send message from the owner to a specific customer
    public async Task SendMessageToCustomer(string customerId, string message)
    {
        if (UserConnections.TryGetValue(customerId, out var customerConnectionId))
        {
            await Clients.Client(customerConnectionId).SendAsync("ReceiveMessage", "Owner", message);
        }
    }
}

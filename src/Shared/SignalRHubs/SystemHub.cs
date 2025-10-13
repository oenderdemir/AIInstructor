using Microsoft.AspNetCore.SignalR;

namespace AIInstructor.src.Shared.SignalRHubs
{
    public sealed class SystemHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            // İsterseniz connection bazlı log tutabilirsiniz
            return base.OnConnectedAsync();
        }
    }
}

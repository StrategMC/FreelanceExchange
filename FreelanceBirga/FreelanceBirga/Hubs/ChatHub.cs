using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    public async Task JoinChat(int chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
    }

    public async Task SendMessage(int chatId, bool sender, string content)
    {
        var message = new
        {
            ChatId = chatId,
            Sender = sender,
            Content = content,
            SendTime = DateTime.Now.ToString("g"),
            IsCurrentUser = false
        };

        await Clients.Group($"chat-{chatId}").SendAsync("ReceiveMessage", message);
    }

    public async Task UpdateChatStatus(int chatId, int newStatus)
    {
        await Clients.Group($"chat-{chatId}").SendAsync("StatusUpdated", newStatus);
    }
}
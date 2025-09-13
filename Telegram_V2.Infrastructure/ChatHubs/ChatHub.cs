using Microsoft.AspNetCore.SignalR;
using Telegram_V2.Infrastructure.Database;
using Telegram_V2.Core.Models;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Telegram_V2.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Context _context;
        private static readonly ConcurrentDictionary<int, string> _connections = new();

        public ChatHub(Context context)
        {
            _context = context;
        }

        // ✅ Ulanuvchi foydalanuvchini ro‘yxatdan o‘tkazadi
        public Task RegisterUser(int userId)
        {
            _connections[userId] = Context.ConnectionId;
            Console.WriteLine($"User {userId} connected with {Context.ConnectionId}");
            return Task.CompletedTask;
        }

        // ✅ 1:1 xabar yuborish
        public async Task SendPrivateMessage(int senderId, int receiverId, string messageText)
        {
            var message = new Message
            {
                ChatId = 0, // One-to-one chat uchun alohida ChatId keyinchalik generate qilinadi
                SenderId = senderId,
                Text = messageText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            if (_connections.TryGetValue(receiverId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", senderId, messageText, message.CreatedAt);
            }
        }

        // ✅ Guruhga qo‘shilish
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("SystemMessage", $"User {Context.ConnectionId} joined {groupName}");
        }

        // ✅ Guruhga xabar yuborish
        public async Task SendGroupMessage(int senderId, string groupName, string messageText)
        {
            var chat = _context.Chats.FirstOrDefault(c => c.Id.ToString() == groupName);
            if (chat == null) return;

            var message = new Message
            {
                ChatId = chat.Id,
                SenderId = senderId,
                Text = messageText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", senderId, messageText, message.CreatedAt);
        }

        // ✅ Foydalanuvchi uzilganda mappingni o‘chirib tashlaymiz
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (user.Key != 0)
            {
                _connections.TryRemove(user.Key, out _);
                Console.WriteLine($"User {user.Key} disconnected.");
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}

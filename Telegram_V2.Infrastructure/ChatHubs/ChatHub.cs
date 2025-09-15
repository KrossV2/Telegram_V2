using Microsoft.AspNetCore.SignalR;
using Telegram_V2.Infrastructure.Database;
using Telegram_V2.Core.Models;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Telegram_V2.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Context _context;
        private static readonly ConcurrentDictionary<int, HashSet<string>> _userConnections = new();

        public ChatHub(Context context)
        {
            _context = context;
        }

        // ✅ Foydalanuvchini ro'yxatdan o'tkazish
        public async Task RegisterUser(int userId)
        {
            // User connections ni yangilash
            _userConnections.AddOrUpdate(userId,
                new HashSet<string> { Context.ConnectionId },
                (key, existingSet) =>
                {
                    existingSet.Add(Context.ConnectionId);
                    return existingSet;
                });

            // User statusini online qilish
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = true;
                user.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            Console.WriteLine($"User {userId} connected with {Context.ConnectionId}");
        }

        // ✅ 1:1 xabar yuborish (YAXSHILANGAN)
        public async Task SendPrivateMessage(int senderId, int receiverId, string messageText)
        {
            // Avval 1:1 chatni topish yoki yaratish
            var chat = await FindOrCreatePrivateChat(senderId, receiverId);

            var message = new Message
            {
                ChatId = chat.Id,
                SenderId = senderId,
                Text = messageText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Receiverning barcha connectionlariga xabar yuborish
            if (_userConnections.TryGetValue(receiverId, out var receiverConnections))
            {
                foreach (var connectionId in receiverConnections)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage",
                        senderId, messageText, message.CreatedAt, message.Id);
                }
            }

            // Senderni o'ziga ham yuborish (message sent confirmation)
            if (_userConnections.TryGetValue(senderId, out var senderConnections))
            {
                foreach (var connectionId in senderConnections)
                {
                    await Clients.Client(connectionId).SendAsync("MessageSent", message.Id);
                }
            }
        }

        // ✅ Guruhga xabar yuborish (YAXSHILANGAN)
        public async Task SendGroupMessage(int senderId, int chatId, string messageText)
        {
            var message = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Text = messageText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Guruh a'zolariga xabar yuborish
            await Clients.Group($"chat_{chatId}").SendAsync("ReceiveGroupMessage",
                senderId, messageText, message.CreatedAt, message.Id);

            // Senderni o'ziga ham yuborish
            if (_userConnections.TryGetValue(senderId, out var senderConnections))
            {
                foreach (var connectionId in senderConnections)
                {
                    await Clients.Client(connectionId).SendAsync("MessageSent", message.Id);
                }
            }
        }

        // ✅ Guruhga qo'shilish (YAXSHILANGAN)
        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{chatId}");
            Console.WriteLine($"User joined chat {chatId}");
        }

        // ✅ Guruhdan chiqish
        public async Task LeaveChat(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{chatId}");
            Console.WriteLine($"User left chat {chatId}");
        }

        // ✅ Foydalanuvchi uzilganda
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Connectionni olib tashlash
            foreach (var user in _userConnections)
            {
                if (user.Value.Contains(Context.ConnectionId))
                {
                    user.Value.Remove(Context.ConnectionId);

                    // Agar boshqa connection qolmagan bo'lsa, user offline
                    if (user.Value.Count == 0)
                    {
                        var userEntity = await _context.Users.FindAsync(user.Key);
                        if (userEntity != null)
                        {
                            userEntity.IsOnline = false;
                            userEntity.LastSeen = DateTime.UtcNow;
                            await _context.SaveChangesAsync();
                        }
                    }
                    break;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // ✅ Yordamchi metod: 1:1 chatni topish yoki yaratish
        private async Task<Chat> FindOrCreatePrivateChat(int user1Id, int user2Id)
        {
            // Mavjud chatni qidirish
            var existingChat = await _context.Chats
                .Where(c => !c.IsGroup)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Participants.Any(p => p.UserId == user1Id) &&
                                         c.Participants.Any(p => p.UserId == user2Id));

            if (existingChat != null)
                return existingChat;

            // Yangi chat yaratish
            var newChat = new Chat
            {
                IsGroup = false,
                CreatedById = user1Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();

            // Participantlarni qo'shish
            _context.ChatParticipants.AddRange(
                new ChatParticipant { ChatId = newChat.Id, UserId = user1Id, Role = "member", JoinedAt = DateTime.UtcNow },
                new ChatParticipant { ChatId = newChat.Id, UserId = user2Id, Role = "member", JoinedAt = DateTime.UtcNow }
            );

            await _context.SaveChangesAsync();
            return newChat;
        }
    }
}
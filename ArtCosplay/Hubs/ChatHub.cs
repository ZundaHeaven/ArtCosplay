using ArtCosplay.Data;
using ArtCosplay.Data.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Data.Entity;
using System.Linq;

public class ChatHub : Hub
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _appDbContext;

    public ChatHub(UserManager<User> userManager, AppDbContext appDbContext)
    {
        _userManager = userManager;
        _appDbContext = appDbContext;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected: {Context.ConnectionId} - {exception?.Message}");
        await base.OnDisconnectedAsync(exception);
    }

    [Authorize]
    public async Task SendMessage(string message, int productId, int? chatId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(Context.User);

            var product = _appDbContext.Products
                .FirstOrDefault(x => x.ProductId == productId);

            if(product == null)
                throw new HubException("Product is null");

            bool isSeller = product.SellerId == user.Id;

            Console.WriteLine("LOGLOGLOGLOGLOGLOGLOGLOGLOGLOGLOG " + chatId);

            Chat? chat;
            if (chatId == null)
                chat = _appDbContext.Chats.FirstOrDefault(x => x.ProductId == product.ProductId &&
                    (x.BuyerId == user.Id || x.SellerId == user.Id));
            else
                chat = _appDbContext.Chats.FirstOrDefault(x => x.ChatId == chatId);

            if (chat == null)
            {
                if (isSeller)
                    throw new HubException("Cant send message yourself");

                chat = new Chat
                {
                    BuyerId = user.Id,
                    SellerId = product.SellerId,
                    ProductId = productId,
                };

                chat = _appDbContext.Chats.Add(chat).Entity;
                _appDbContext.SaveChanges();
            }

            _appDbContext.Messages.Add(new Message
            {
                ChatId = chat.ChatId,
                Content = message,
                SenderId = user.Id
            });
            _appDbContext.SaveChanges();


            if (string.IsNullOrEmpty(message))
                throw new HubException("Message cannot be empty");

            await Clients.All.SendAsync("ReceiveMessage", message, user.Id, chat.ChatId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hub Error: {ex}");
            throw;
        }
    }
}
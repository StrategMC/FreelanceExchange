using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using FreelanceBirga.Models.SM;
using FreelanceBirga.Models.VM;
using FreelanceBirga.Models.DB;
using System.Diagnostics.Eventing.Reader;

public class ChatController : Controller
{
    private readonly AppDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;
    int? userId;
    int? customerId;
    int? executorId;

    public ChatController(AppDbContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    private async Task LoadUserData()
    {
        userId = HttpContext.Session.GetInt32("UserId");
        if (userId.HasValue)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserID == userId);
            var executor = await _context.Executors.FirstOrDefaultAsync(e => e.UserID == userId);

            
            customerId = customer?.Id;
            executorId = executor?.Id;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Chat(int chatId)
    {
        Console.WriteLine($"chat: {chatId}");
        await LoadUserData();
        if (!userId.HasValue)
        {
           
            return RedirectToAction("Index", "Home");
        }

        var chat = await _context.OrdersChat
            .FirstOrDefaultAsync(c => c.Id == chatId);
     
        if (chat == null || (customerId == null && executorId == null))
        {
            return RedirectToAction("Index", "Home");
        }

        bool hasAccess = (customerId != null && chat.CustomerId == customerId) ||
                        (executorId != null && chat.ExecutorId == executorId);

        if (!hasAccess)
        {
            return RedirectToAction("Index", "Home");
        }

        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SendTime)
            .ToListAsync();
        var order = await _context.Orders.FindAsync(chat.OrderId);
        var model = new ChatViewModel
        {
            ChatId = chat.Id,
            OrderId = chat.OrderId,
            OrderName = order.Title,
            CurrentUserId = (int)userId,
            IsCustomer = customerId != null,
            Status = chat.Status,
            Messages = messages.Select(m => new MessageViewModel
            {
                Content = m.Content,
                Sender = m.Sender,
                SendTime = m.SendTime,
                IsCurrentUser = (m.Sender && customerId != null) ||
                              (!m.Sender && executorId != null)
            }).ToList(),
            ShowReviewButton = await ShouldShowReviewButton(chat, userId.Value)
        };

        return View(model);
    }
    async Task<bool> ShouldShowReviewButton(OrdersChat chat, int userId)
    {
        bool have_chatforreview = await _context.OrdersChatForRewiew.AnyAsync(ord => ord.OrderChatId == chat.Id);
        if (!have_chatforreview) 
        {
            return false;
        }
        bool user_have_review = true;
        if (executorId != null)
        {
            user_have_review = await _context.ReviewsExecutor.AnyAsync(rw => rw.OrderId == chat.OrderId && rw.RecipientId == chat.CustomerId);
        }
        else if(customerId != null) 
        {
            user_have_review = await _context.ReviewsCustomer.AnyAsync(rw => rw.OrderId == chat.OrderId && rw.RecipientId == chat.ExecutorId);
        }
        if (!user_have_review)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await LoadUserData();
        if (!userId.HasValue)
        {
            return Json(new { success = false, error = "Требуется авторизация" });
        }

        var chat = await _context.OrdersChat.FindAsync(model.ChatId);

        if (chat == null || chat.Status > 1)
            return Json(new { success = false, error = "Чат недоступен" });

        bool isParticipant = (customerId != null && chat.CustomerId == customerId) ||
                           (executorId != null && chat.ExecutorId == executorId);

        if (!isParticipant)
            return Json(new { success = false, error = "Нет доступа" });

        var message = new Message
        {
            ChatId = model.ChatId,
            Content = model.Content.Trim(),
            Sender = customerId != null,
            SendTime = DateTime.Now
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group($"chat-{model.ChatId}")
            .SendAsync("ReceiveMessage", new
            {
                message.ChatId,
                message.Sender,
                message.Content,
                SendTime = message.SendTime.ToString("g"),
                IsCurrentUser = false
            });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await LoadUserData();
        if (!userId.HasValue)
        {
            return Json(new { success = false, error = "Требуется авторизация" });
        }

        var chat = await _context.OrdersChat.FindAsync(model.ChatId);

        if (chat == null)
            return Json(new { success = false, error = "Чат не найден" });

        bool canChangeStatus = (customerId != null && (
                                  (model.NewStatus == 1 && chat.Status == 0) ||
                                  (model.NewStatus == 2 && chat.Status == 1) ||
                                  (model.NewStatus == 3))) ||
                              (executorId != null && model.NewStatus == 3 && chat.Status == 1);

        if (!canChangeStatus)
            return Json(new { success = false, error = "Невозможно изменить статус" });
        var order_for_chek = await _context.Orders.FindAsync(chat.OrderId);
        if ((order_for_chek.ExecutorID != chat.ExecutorId && order_for_chek.ExecutorID != null) && model.NewStatus == 1)
        {
            return Json(new { success = false, error = "Вы уже отдали этот заказ другому исполнителю" });
        }
        chat.Status = model.NewStatus;
        _context.Update(chat);
        switch (model.NewStatus)
        {
            case 1:
                {
                    var order = await _context.Orders.FindAsync(chat.OrderId);
                    order.ExecutorID = chat.ExecutorId;
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine("-");
                        Console.WriteLine($"ExecutorID: {executorId}");
                    }
                    order.InWork = true;
                    OrderChatForRewiew orderChatForRewiew = new OrderChatForRewiew
                    {
                        OrderChatId = chat.Id
                    };
                    _context.OrdersChatForRewiew.Add(orderChatForRewiew);
                }
                break;
            case 2:
                {
                    var order = await _context.Orders.FindAsync(chat.OrderId);
                    order.Redy = true;
                }
                break;
            case 3:
                {
                    var order = await _context.Orders.FindAsync(chat.OrderId);
                    if(order.ExecutorID == chat.ExecutorId)
                    {
                        order.ExecutorID = null;
                        order.InWork = false;
                        OrderChatForRewiew orderChatForRewiew = new OrderChatForRewiew
                        {
                            OrderChatId = chat.Id
                        };
                        _context.OrdersChatForRewiew.Add(orderChatForRewiew);
                    }
                }
                break;
            default:
                break;
        }

        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group($"chat-{model.ChatId}")
            .SendAsync("StatusUpdated", model.NewStatus);
        

        return Json(new { success = true });
    }
}
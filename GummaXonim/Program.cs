using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

class Program
{
    private static readonly string Token = "7454403637:AAHOC189FNUVNfIwdBklpZdbvJoTnfT2IeU"; // BotFather bergan tokenni shu yerga qo'ying
    private static readonly TelegramBotClient BotClient = new TelegramBotClient(Token);

    static async Task Main()
    {
        Console.WriteLine("Bot ishga tushdi...");

        using var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message } // Faqat xabarlarni qabul qiladi
        };

        BotClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cts.Token);

        Console.ReadLine();
        cts.Cancel();
    }

    private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message || message.Text is not { } messageText)
            return;

        long chatId = message.Chat.Id;

        if (messageText == "/start")
        {
            await bot.SendTextMessageAsync(chatId, "Xush kelibsiz!", cancellationToken: cancellationToken);
        }
    }

    private static Task ErrorHandler(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Xatolik yuz berdi: {exception.Message}");
        return Task.CompletedTask;
    }
}

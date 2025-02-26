using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static readonly string BotToken = "7454403637:AAHOC189FNUVNfIwdBklpZdbvJoTnfT2IeU"; // BotFather dan olingan token

    static async Task Main(string[] args)
    {
        // Web serverni ishga tushurish
        var webHost = CreateHostBuilder(args).Build();

        // Telegram botni ishga tushurish
        var botClient = new TelegramBotClient(BotToken);
        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Bot {me.Username} ishga tushdi!");

        botClient.StartReceiving(UpdateHandler, ErrorHandler);

        // Web serverni ishga tushirish
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        webHost.RunAsync($"http://0.0.0.0:{port}");

        Console.WriteLine("Bot ishlayapti. Chiqish uchun Enter tugmasini bosing.");
        Console.ReadLine();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://0.0.0.0:8080") // Portni o'zgartirish mumkin
                          .UseStartup<Startup>();
            });

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Foydalanuvchi xabariga javob qaytarish
            switch (messageText.ToLower())
            {
                case "/start":
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "O'zbekiston tarixiy obidalari botiga xush kelibsiz! Quyidagi buyruqlardan birini tanlang:\n\n" +
                              "/obidalar - Tarixiy obidalar ro'yxati\n" +
                              "/about - Bot haqida ma'lumot",
                        cancellationToken: cancellationToken);
                    break;

                case "/obidalar":
                    var replyMarkup = new InlineKeyboardMarkup(new[] 
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("Registon maydoni", "registon") },
                        new[] { InlineKeyboardButton.WithCallbackData("Bibi Xonim masjidi", "bibi_xonim") },
                        new[] { InlineKeyboardButton.WithCallbackData("Shahi Zinda", "shahi_zinda") }
                    });

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Quyidagi obidalardan birini tanlang:",
                        replyMarkup: replyMarkup,
                        cancellationToken: cancellationToken);
                    break;

                default:
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Noto'g'ri buyruq. Iltimos, /start ni bosing.",
                        cancellationToken: cancellationToken);
                    break;
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var data = callbackQuery.Data;

            string responseText = data switch
            {
                "registon" => "Registon maydoni - Samarqand shahridagi mashhur tarixiy obida. 3 ta madrasa (Ulugʻbek, Sherdor va Tilla Qori)dan iborat.",
                "bibi_xonim" => "Bibi Xonim masjidi - Amir Temur tomonidan qurilgan ulkan masjid. Samarqandning ramzlaridan biri.",
                "shahi_zinda" => "Shahi Zinda - Samarqanddagi qadimiy qabriston va me'moriy yodgorliklar majmuasi.",
                _ => "Noto'g'ri tanlov."
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: responseText,
                cancellationToken: cancellationToken);
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}

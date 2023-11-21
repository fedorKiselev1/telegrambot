using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("6753781529:AAHdnjPFm_3cAXgm3Ax2TMkz4ig-hvSFPgE");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    if (messageText == "Привет")
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "приветы",
            cancellationToken: cancellationToken);
    }

    if (messageText == "Картинка")
    {
        await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: InputFile.FromUri("https://img.freepik.com/free-photo/a-picture-of-fireworks-with-a-road-in-the-background_1340-43363.jpg"),
            cancellationToken: cancellationToken);
    }

    if (messageText == "Видео")
    {
        await botClient.SendVideoAsync(
            chatId: chatId,
            video: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4"),
            supportsStreaming: true,
            cancellationToken: cancellationToken);
    }

    if (messageText == "Стикер")
    {
        await botClient.SendStickerAsync(
            chatId: chatId,
            sticker: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/sticker-fred.webp"),
            cancellationToken: cancellationToken);
    }

    if (messageText == "Кнопка")
    {
        Message pollMessage = await botClient.SendPollAsync(
        chatId: chatId,
        question: "да или нет",
        options: new[]
        {
            "да",
            "нед"
        },
        cancellationToken: cancellationToken);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
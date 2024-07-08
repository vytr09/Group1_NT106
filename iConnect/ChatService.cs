using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using FireSharp.Response;

public class ChatService
{
    public IFirebaseClient Client { get; }

    private long _lastTimestamp;

    public ChatService(IFirebaseClient firebaseClient)
    {
        Client = firebaseClient;
        _lastTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public async Task SendMessageAsync(string chatId, string sender, string receiver, string text)
    {
        var message = new ChatMessage
        {
            Sender = sender,
            Receiver = receiver,
            Text = text,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        try
        {
            await Client.PushAsync($"Messages/{chatId}", message);
            Console.WriteLine($"Message sent: {message.Text}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public async Task<List<ChatMessage>> ObserveMessages(string chatId)
    {
        try
        {
            FirebaseResponse response = await Client.GetAsync($"Messages/{chatId}");
            if (response.Body != "null")
            {
                var messages = response.ResultAs<Dictionary<string, ChatMessage>>();
                return messages != null ? new List<ChatMessage>(messages.Values) : new List<ChatMessage>();
            }
            return new List<ChatMessage>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error observing messages: {ex.Message}");
            return new List<ChatMessage>();
        }
    }

    public async Task PollForMessages(string chatId, Action<ChatMessage> onNewMessage)
    {
        while (true)
        {
            try
            {
                FirebaseResponse response = await Client.GetAsync($"Messages/{chatId}");
                if (response.Body != "null")
                {
                    var messages = response.ResultAs<Dictionary<string, ChatMessage>>();
                    var newMessages = messages.Values.Where(m => m.Timestamp > _lastTimestamp).ToList();
                    foreach (var message in newMessages)
                    {
                        onNewMessage(message);
                        _lastTimestamp = message.Timestamp;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error polling messages: {ex.Message}");
            }

            await Task.Delay(1000); // Poll every 1 second
        }
    }
}

public class ChatMessage
{
    public string Sender { get; set; }
    public string Receiver { get; set; }
    public string Text { get; set; }
    public long Timestamp { get; set; }
}

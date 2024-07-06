using System;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Collections.Generic;

public class ChatService
{
    private readonly IFirebaseClient _firebaseClient;

    public ChatService(IFirebaseClient firebaseClient)
    {
        _firebaseClient = firebaseClient;
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
            await _firebaseClient.PushAsync($"Messages/{chatId}", message);
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
            FirebaseResponse response = await _firebaseClient.GetAsync($"Messages/{chatId}");
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
}

public class ChatMessage
{
    public string Sender { get; set; }
    public string Receiver { get; set; }
    public string Text { get; set; }
    public long Timestamp { get; set; }
}

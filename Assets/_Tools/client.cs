public class client
{
    /// <summary>
    /// Sends bot messages/whispers to the main queue.
    /// </summary>
    /// <param name="message"></param>
    public static void SendMessage(string message, Timers timer = Timers.Null) => MessageQueue.messageQueue.Enqueue(new Message(message, timer));
    public static void SendWhisper(string username, string message, Timers timer = Timers.Null) => MessageQueue.messageQueue.Enqueue(new Message(username, message, timer));

    /// <summary>
    /// Sends bot messages/whispers to the mod queue.
    /// This is emptied first, as mod commands can affect the game.
    /// </summary>
    /// <param name="message"></param>
    public static void SendModMessage(string message, Timers timer = Timers.Null) => MessageQueue.modQueue.Enqueue(new Message(message, timer));
    public static void SendModWhisper(string username, string message, Timers timer = Timers.Null) => MessageQueue.modQueue.Enqueue(new Message(username, message, timer));

    /// <summary>
    /// Once the messages are dequeued, they are sent through here to the Twitch Client.
    /// </summary>
    /// <param name="message"></param>
    public static void SendMessageQueued(string message) => TwitchConnection.Instance.SendMessage(message);
    public static void SendWhisperQueued(string username, string message) => TwitchConnection.Instance.SendWhisper(username, message);
}
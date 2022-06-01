using System.Collections.Generic;

namespace CommonPlugin
{
    public static class MessageQueue
    {
        public static Dictionary<int, PersonMessage> Messages { get; set; }
        static MessageQueue() => Messages = new Dictionary<int, PersonMessage>();
    }

    public class Message
    {
        public string Text { get; set; }
        public int Duration { get; set; }

        public Message(string text, int duration = 5)
        {
            this.Text = text;
            this.Duration = duration;
        }
    }

    public class PersonMessage
    {
        public int PlayerId { get; set; }
        public ReferenceHub Hub { get; set; }
        public List<Message> TextDisplay { get; set; }

        public PersonMessage(int plyId, ReferenceHub hub)
        {
            this.PlayerId = plyId;
            this.Hub = hub;
            this.TextDisplay = new List<Message>();
        }
    }

    public enum MessageType
    {
        All,
        Person,
        TeamMtf,
        TeamChaos,
        TeamScp,
        TeamSpectator,
        AdminChat
    }
}
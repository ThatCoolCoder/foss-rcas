using Godot;
using System;
using System.Linq;

namespace UI
{

    public class MessageManager : Control
    {
        public static MessageManager Instance { get; private set; }
        private PackedScene messageBoxScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/MessageBox.tscn");

        public void AddMessage(Message message)
        {
            // Check if there is already a message in this category
            var existingNodes = this.GetChildNodeList()
                .Where(x => x is MessageBox)
                .Select(x => x as MessageBox);
            var existingNodeInCategory = existingNodes.FirstOrDefault(x => x.Message.Category == message.Category);

            // If there is no node, create one
            if (existingNodeInCategory == null)
            {
                var instance = messageBoxScene.Instance<MessageBox>();
                AddChild(instance);
                instance.SetMessage(message);
            }
            else
            {
                existingNodeInCategory.SetMessage(message);
            }
        }

        public override void _EnterTree()
        {
            Instance = this;
        }

        public override void _ExitTree()
        {
            Instance = null;
        }

        public static void StaticAddMessage(Message notification)
        {
            if (Instance != null) Instance.AddMessage(notification);
        }
    }

    public class Message
    {
        public string Category { get; set; } = Guid.NewGuid().ToString(); // used to group notifications
        public string Content { get; set; } = "No content provided!";
        public float TimeDisplayed { get; set; } = 5;

        public Message() { }

        public Message(string content, string category = null, float? timeDisplayed = null)
        {
            Content = content;
            if (category != null) Category = category;
            if (timeDisplayed != null) TimeDisplayed = (float)timeDisplayed;
        }
    }
}
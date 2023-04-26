using Godot;
using System;
using System.Linq;

namespace UI
{

    public class NotificationManager : Control
    {
        public static NotificationManager Instance { get; private set; }
        private PackedScene notificationBoxScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/NotificationBox.tscn");

        public void AddNotification(Notification notification)
        {
            // Check if there is already a notification in this category
            var existingNodes = this.GetChildNodeList()
                .Where(x => x is NotificationBox)
                .Select(x => x as NotificationBox);
            var existingNodeInCategory = existingNodes.FirstOrDefault(x => x.Notification.Category == notification.Category);

            // If there is no node, create one
            if (existingNodeInCategory == null)
            {
                var instance = notificationBoxScene.Instance<NotificationBox>();
                AddChild(instance);
                instance.SetNotification(notification);
            }
            else
            {
                existingNodeInCategory.SetNotification(notification);
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

        public static void StaticAddNotification(Notification notification)
        {
            if (Instance != null) Instance.AddNotification(notification);
        }
    }

    public class Notification
    {
        public string Category { get; set; } = Guid.NewGuid().ToString(); // used to group notifications
        public string Content { get; set; } = "No content provided!";
        public float TimeDisplayed { get; set; } = 5;

        public Notification() { }

        public Notification(string content, string category = null, float? timeDisplayed = null)
        {
            Content = content;
            if (category != null) Category = category;
            if (timeDisplayed != null) TimeDisplayed = (float)timeDisplayed;
        }
    }
}
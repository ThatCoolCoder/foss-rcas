using Godot;
using System;
using System.Linq;

namespace UI;

public partial class NotificationManager : Node
{
    public static NotificationManager Instance { get; private set; } // the only reason we make this a node is so that we have reference to the scene tree easily

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        Instance = null;
    }

    public static void AddNotification(UserNotification notification)
    {
        if (Instance == null)
        {
            throw new Exception("Cannot add notification as there is no MotificationManager in the tree");
        }

        foreach (var holder in Instance.GetTree().GetNodesInGroup("NotificationHolder"))
        {
            (holder as UI.Apps.NotificationHolder).AddNotification(notification);
        }
    }

    public static void AddNotification(string content, string category = null, float? timeDisplayed = null)
    {
        AddNotification(new UserNotification(content, category, timeDisplayed));
    }
}

public class UserNotification
{
    public string Category { get; set; } = Guid.NewGuid().ToString(); // used to group notifications
    public string Content { get; set; } = "No content provided!";
    public float TimeDisplayed { get; set; } = 5;

    public UserNotification() { }

    public UserNotification(string content, string category = null, float? timeDisplayed = null)
    {
        Content = content;
        if (category != null) Category = category;
        if (timeDisplayed != null) TimeDisplayed = (float)timeDisplayed;
    }
}
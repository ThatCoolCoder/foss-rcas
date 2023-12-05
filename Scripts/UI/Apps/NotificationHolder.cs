using Godot;
using System;
using System.Linq;

namespace UI.Apps;

[Tool]
[GlobalClass]
public partial class NotificationHolder : Misc.UserManipulate
{
    // Thing that holds all the NotificationBoxes

    private PackedScene notificationBoxScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Apps/NotificationBox.tscn");

    public override void _Ready()
    {
        AddToGroup("NotificationHolder");
        base._Ready();
    }

    public void AddNotification(UserNotification notification)
    {
        // Check if there is already a message in this category
        var existingNodes = this.GetChildNodeList()
            .Where(x => x is NotificationBox)
            .Select(x => x as NotificationBox);
        var existingNodeInCategory = existingNodes.FirstOrDefault(x => x.Notification.Category == notification.Category);

        // If there is no node, create one
        if (existingNodeInCategory == null)
        {
            var instance = notificationBoxScene.Instantiate<NotificationBox>();
            AddChild(instance);
            instance.SetMessage(notification);
        }
        else
        {
            existingNodeInCategory.SetMessage(notification);
        }
    }
}
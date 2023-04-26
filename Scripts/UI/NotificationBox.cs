using Godot;
using System;

namespace UI
{
    public class NotificationBox : Control
    {
        // Todo: we should call notifications something else since they clash with an existing godot thing
        public new Notification Notification { get; private set; }
        private Timer deleteTimer;
        private Label label;
        private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            deleteTimer = GetNode<Timer>("DeleteTimer");
            label = GetNode<Label>("MarginContainer/Label");
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }

        public void SetNotification(Notification _notification)
        {
            Notification = _notification;
            label.Text = Notification.Content;
            deleteTimer.Start(Notification.TimeDisplayed);
        }

        private void _on_DeleteTimer_timeout()
        {
            animationPlayer.Play("FadeOut");
        }

        public void _on_AnimationPlayer_animation_finished(string _)
        {
            QueueFree();
        }
    }
}
using Godot;
using System;

namespace UI
{
    public partial class MessageBox : Control
    {
        public Message Message { get; private set; }
        private Timer deleteTimer;
        private Label label;
        private AnimationPlayer animationPlayer;

        public override void _Ready()
        {
            deleteTimer = GetNode<Timer>("DeleteTimer");
            label = GetNode<Label>("MarginContainer/Label");
            animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        }

        public void SetMessage(Message _message)
        {
            Message = _message;
            label.Text = Message.Content;
            deleteTimer.Start(Message.TimeDisplayed);
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
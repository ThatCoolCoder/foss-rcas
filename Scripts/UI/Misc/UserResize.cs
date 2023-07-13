using Godot;
using System;

namespace UI.Misc;

[Tool]
public partial class UserResize : Control
{
    // Control that can be moved and resized by user.

    [Export] public bool Resizable { get; set; } = true;
    [Export] public bool Movable { get; set; } = true;
    [Export] public Control BackgroundNode { get; set; }
    [Export] public int DraggerSize { get; set; } = 16;
    [Export] public Texture2D NormalCornerTexture { get; set; }
    [Export] public Texture2D LockedCornerTexture { get; set; }
    [Export] public Vector2I CornerTextureSize { get; set; }
    [Export] public Vector2I MinDragSize { get; set; } = Vector2I.One * 100;

    private Control dragger;
    private TextureRect cornerTexture;
    private Vector2? clickStartPos;
    private Vector2 selfPosAtClickStart;

    public override void _Ready()
    {
        // not gotten through direct node exports as then it will show
        dragger = GetNode<Control>("Dragger");
        cornerTexture = GetNode<TextureRect>("CornerTexture");

        if (Engine.IsEditorHint()) return;

        if (BackgroundNode != null)
        {
            BackgroundNode.MouseFilter = MouseFilterEnum.Pass;
            MoveChild(BackgroundNode, 0); // needs to be behind others for input and view
        }
    }

    public override void _Process(double _delta)
    {
        if (Engine.IsEditorHint())
        {
            cornerTexture.Size = Vector2I.One * DraggerSize;
            cornerTexture.OffsetLeft = -DraggerSize;
            cornerTexture.OffsetTop = -DraggerSize;

            dragger.Size = Vector2I.One * DraggerSize;
            dragger.OffsetLeft = -DraggerSize;
            dragger.OffsetTop = -DraggerSize;
        }

        dragger.MouseDefaultCursorShape = Resizable ? CursorShape.Fdiagsize : CursorShape.Arrow;
        cornerTexture.Texture = Resizable ? NormalCornerTexture : LockedCornerTexture;
    }

    public void _on_Dragger_gui_input(InputEvent _event)
    {
        if (Engine.IsEditorHint()) return;

        if (Resizable && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Size = GetLocalMousePosition();
        }
    }

    public override void _GuiInput(InputEvent _event)
    {
        if (Engine.IsEditorHint() || !Movable) return;

        if (_event is InputEventMouseButton buttonEvent)
        {
            if (buttonEvent.Pressed)
            {
                clickStartPos = GetGlobalMousePosition();
                selfPosAtClickStart = Position;
            }
            else
            {
                clickStartPos = null;
            }
        }
        else if (_event is InputEventMouseMotion motionEvent)
        {
            if (clickStartPos != null)
            {
                Position = GetGlobalMousePosition() - (Vector2)clickStartPos + selfPosAtClickStart;
            }
        }
    }
}
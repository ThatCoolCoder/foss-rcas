using Godot;
using System;

namespace UI.Misc;

[Tool]
[GlobalClass]
public partial class UserManipulate : Control
{
    // Control that can be moved and resized by user.

    [Export] public bool Resizable { get; set; } = true;
    [Export] public bool Movable { get; set; } = true;
    [Export] public bool Deletable { get; set; } = true;
    [Export] public bool AutoAdjustAnchors { get; set; } = true;
    [Export] public Control BackgroundNode { get; set; }
    [Export] public int CornerButtonSize { get; set; } = 16;
    [Export] public Texture2D ResizeButtonTexture { get; set; }
    [Export] public Texture2D DeleteButtonTexture { get; set; }
    [Export] public Vector2I MinDragSize { get; set; } = Vector2I.One * 100;
    [Signal] public delegate void OnDeletedEventHandler(UserManipulate userManipulate);

    private Control resizeArea;
    private TextureRect resizeTexture;
    private Vector2? clickStartPos;
    private Vector2 selfPosAtClickStart;
    private TextureButton deleteButton;

    public override void _Ready()
    {
        // not gotten through direct node exports as this is internal and 
        resizeArea = GetNode<Control>("ResizeArea");
        resizeTexture = GetNode<TextureRect>("ResizeTexture");
        deleteButton = GetNode<TextureButton>("DeleteButton");

        CustomMinimumSize = MinDragSize;

        if (Engine.IsEditorHint()) return;

        if (BackgroundNode != null)
        {
            BackgroundNode.MouseFilter = MouseFilterEnum.Pass;
            MoveChild(BackgroundNode, 0); // needs to be behind others for input and appearance
        }
    }

    public override void _Process(double _delta)
    {
        if (Engine.IsEditorHint())
        {
            resizeTexture.Size = Vector2I.One * CornerButtonSize;
            resizeTexture.OffsetLeft = -CornerButtonSize;
            resizeTexture.OffsetTop = -CornerButtonSize;

            resizeArea.Size = Vector2I.One * CornerButtonSize;
            resizeArea.OffsetLeft = -CornerButtonSize;
            resizeArea.OffsetTop = -CornerButtonSize;
            resizeTexture.Texture = Resizable ? ResizeButtonTexture : null;

            deleteButton.Size = Vector2I.One * CornerButtonSize;
            deleteButton.OffsetLeft = -CornerButtonSize;
            deleteButton.OffsetBottom = CornerButtonSize;
            deleteButton.TextureNormal = DeleteButtonTexture;
        }

        resizeArea.MouseDefaultCursorShape = Resizable ? CursorShape.Fdiagsize : CursorShape.Arrow;
        resizeTexture.Visible = Resizable;
        deleteButton.Visible = Deletable;
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

                if (AutoAdjustAnchors)
                {

                    var selfSize = Size;
                    var parentSize = GetViewportRect().Size;
                    if (GetParent() is Control control)
                    {
                        parentSize = control.Size;
                    }

                    AnchorLeft = SelectAnchorPosition(Position.X + selfSize.X / 2, parentSize.X);
                    AnchorRight = AnchorLeft;

                    AnchorTop = SelectAnchorPosition(Position.Y + selfSize.Y / 2, parentSize.Y);
                    AnchorBottom = AnchorTop;

                    Size = selfSize;
                }
            }
        }
    }

    private void _on_DeleteButton_pressed()
    {
        EmitSignal(SignalName.OnDeleted, this);
        QueueFree();
    }

    private float SelectAnchorPosition(float position, float parentSize, float middleProportion = 0.2f)
    {
        var proportion = position / parentSize;
        if (proportion < 0.5f - middleProportion / 2) return 0;
        else if (proportion < 0.5f + middleProportion / 2) return 0.5f;
        else return 1;
    }
}
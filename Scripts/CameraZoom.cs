using Godot;
using System;

public class CameraZoom : Camera
{
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent)
        {

            if (mouseEvent.ButtonIndex == 4) // 4 = up
            {
                Fov -= 1;
            }
            if (mouseEvent.ButtonIndex == 5) // 5 = down
            {
                Fov += 1;
            }
        }
    }
}

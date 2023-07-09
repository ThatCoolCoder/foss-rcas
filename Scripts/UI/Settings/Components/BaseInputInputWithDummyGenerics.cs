using Godot;
using System;

namespace UI.Settings.Components
{
    public abstract partial class BaseInputInputWithDummyGenerics : BaseInputInput<int, int?>
    {
        // Godot gives an error when trying to use a script with generics, even if it is overwritten by the extending scene.
        // So this exists to be used in BaseInputInput.tscn so there are no errors.
    }
}
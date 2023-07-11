using Godot;
using System;

namespace UI.Settings.Components;

public partial class BaseInputInputWithDummyGenerics : BaseInputInput<int, int?>
{
    // Godot gives an error when trying to use a script with generics or abstracts, even if it is overwritten by the extending scene.
    // So this exists to be used in BaseInputInput.tscn so there are no errors.

    protected override void ClearCandidateValue()
    {
        throw new NotImplementedException();
    }

    protected override int? GetCandidateValue()
    {
        throw new NotImplementedException();
    }

    protected override string GetCurrentValueText()
    {
        throw new NotImplementedException();
    }

    protected override string GetPopupText()
    {
        throw new NotImplementedException();
    }
}
using Godot;
using System;

namespace UI
{
    public class LocationSelector : AbstractContentSelector<ContentManagement.Location>
    {
        protected override string FormatCustomInfo()
        {
            return $"Location: {SelectedItem.LocationInWorld}\n" +
                $"Elevation: {SelectedItem.Elevation}m ASL";
        }
    }
}
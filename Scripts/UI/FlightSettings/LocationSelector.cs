using Godot;
using System;

namespace UI.FlightSettings
{
    public partial class LocationSelector : AbstractContentSelector<ContentManagement.Location>
    {
        protected override string FormatCustomInfo()
        {
            return $"Location: {SelectedItem.LocationInWorld}\n" +
                $"Elevation: {SelectedItem.Elevation}m ASL";
        }
    }
}
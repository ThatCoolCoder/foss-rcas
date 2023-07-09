using Godot;
using System;

namespace UI.Apps
{
    public partial class PropellerDebug : AbstractBasicNodeDebug<Physics.Forcers.Propeller>
    {
        protected override string GroupName { get; set; } = "Propeller";

        protected override string GenerateText(Physics.Forcers.Propeller propeller)
        {
            return $"Prop name: {propeller.Name}\n" +
                $"L/D: {propeller.LiftToDrag:F2}\n" +
                $"Efficiency: {propeller.EfficiencyFactor * 100:F0}%\n" +
                $"Mass: {propeller.Mass:F3}Kg\n" +
                $"Clockwise: {propeller.Clockwise}\n" +
                $"Size: {propeller.DiameterInches:F1}x{propeller.PitchInches:F1} in\n" +
                $"Rpm: {propeller.Rpm:F0}\n" +
                $"Thrust: {propeller.LastThrustMagnitude:F2}N ({propeller.LastThrustMagnitude / 9.81f:F3}Kg)";
        }
    }
}
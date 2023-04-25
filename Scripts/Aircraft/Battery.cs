using Godot;
using System;

namespace Aircraft
{
    public class Battery : Spatial
    {
        [Export] public int CellCount { get; set; } = 3;
        [Export] public float ChargedCellVoltage { get; set; } = 4.2f;
        [Export] public float FlatCellVoltage { get; set; } = 3.3f;
        [Export] public float InternalResistance { get; set; } = 0.003f; // todo: calculate voltage sag based on current

        public float CurrentCellVoltage
        {
            get
            {
                if (RemainingCapacity == 0) return 0; // If it is completely flat have the plane just die
                // todo: have a voltage curve when discharging instead of linear
                return Mathf.Lerp(ChargedCellVoltage, FlatCellVoltage, RemainingCapacity / MaxCapacity);
            }
        }
        public float CurrentVoltage
        {
            get
            {
                return CurrentCellVoltage * CellCount;
            }
        }
        [Export] public float MaxCapacity { get; set; } = 1; // amp hours
        public float RemainingCapacity { get; private set; }


        public override void _Ready()
        {
            RemainingCapacity = MaxCapacity;
        }

        public void Discharge(float ampHours)
        {
            RemainingCapacity = Mathf.Max(RemainingCapacity - ampHours, 0);
        }

        public void Discharge(float amps, float seconds)
        {
            Discharge(amps * seconds / 3600);
        }
    }
}

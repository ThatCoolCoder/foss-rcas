using Godot;
using System;

namespace Aircraft
{
    public class Battery : Spatial
    {
        [Export] public int CellCount { get; set; } = 3;
        [Export] public float ChargedCellVoltage { get; set; } = 4.2f;
        [Export] public float FlatCellVoltage { get; set; } = 3;
        [Export] public float CellInternalResistance { get; set; } = 0.003f;
        [Export] public Curve DischargeCurve { get; set; } = ResourceLoader.Load<Curve>("res://Resources/LipoDischargeCurve.tres");

        public float CurrentCellVoltage
        {
            get
            {
                if (RemainingCapacity == 0) return 0; // If it is completely flat have the plane just die

                var capacityUsed = 1 - (RemainingCapacity / MaxCapacity);
                var baseVoltage = Mathf.Lerp(FlatCellVoltage, ChargedCellVoltage, DischargeCurve.Interpolate(capacityUsed));
                baseVoltage -= CurrentDrawn * CellInternalResistance;
                return baseVoltage;
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

        // Amp hours. Used to calculate current and by extension sag
        private float currentUsageAccumulator = 0;
        private float timeSinceLastCurrentCalc = 0;
        private int currentCalculationInterval = 30;
        public float CurrentDrawn { get; private set; }


        public override void _Ready()
        {
            RemainingCapacity = MaxCapacity;
        }

        public override void _Process(float delta)
        {
            // Calculate current.
            // Todo: this and the related fields are a bit messy, please clean this up
            timeSinceLastCurrentCalc += delta;
            if (Engine.GetFramesDrawn() % currentCalculationInterval == 0)
            {
                var dischargeAmount = currentUsageAccumulator;
                currentUsageAccumulator = 0;
                CurrentDrawn = dischargeAmount / (timeSinceLastCurrentCalc / 60 / 60);
                timeSinceLastCurrentCalc = 0;
            }
        }

        public void Discharge(float ampHours)
        {
            var previousRemainingCapacity = RemainingCapacity;
            RemainingCapacity = Mathf.Max(RemainingCapacity - ampHours, 0);
            currentUsageAccumulator += ampHours;
            if (RemainingCapacity == 0 && previousRemainingCapacity > 0)
                UI.NotificationManager.StaticAddNotification(new UI.Notification(category: "battery", content: $"Battery flat"));
        }

        public void Discharge(float amps, float seconds)
        {
            Discharge(amps * seconds / 3600);
        }
    }
}

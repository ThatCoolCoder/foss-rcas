using Godot;
using System;

namespace Physics.Fluids
{
    public class WindSettings
    {
        public float Speed { get; set; } = 0;
        public float GustSpeedDelta { get; set; } = 1.5f; // max gust speed = speed + gust speed delta
        public float GustFrequency { get; set; } = 20;
        public float DirectionDegrees
        {
            get
            {
                return Mathf.Rad2Deg(Direction);
            }
            set
            {
                Direction = Mathf.Deg2Rad(value);
            }
        }
        public float Direction = 3.14f;
        public float DirectionVariabilityDegrees // from 0 to 180
        {
            get
            {
                return Mathf.Rad2Deg(DirectionVariability);
            }
            set
            {
                DirectionVariability = Mathf.Deg2Rad(value);
            }
        }
        public float DirectionVariability = 0.4f;
        public float DirectionChangeFrequency = 30;
    }
}
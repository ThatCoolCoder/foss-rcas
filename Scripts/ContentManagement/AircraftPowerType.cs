using Godot;
using System;

namespace ContentManagement;

public enum AircraftPowerType
{
    ElectricPropeller = 0,
    ElectricDuctedFan = 1,
    InternalCombustion = 2,
    Turbine = 3,
    Other = 4
}
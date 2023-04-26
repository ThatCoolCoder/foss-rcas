using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Tomlet;

namespace SimInput
{
    public abstract class IControlMapping
    {
        // Try to process event that is given. Return null if event isn't bound to the mapping 
        public abstract float? ProcessEvent(InputEvent _event);

        public string TypeName { get { return GetType().Name; } }

        // Tomlet does not appreciate loading lists of interfaces (fair enough), so here is a slightly bodgy custom mapper for IControlMappings 
        // A static constructor is used to register the mapper since Godot doesn't provide access to the entry point of the project
        // Perhaps this should be somewhere in the SimSettings namespace since that's where all the other persistence stuff is
        static IControlMapping()
        {
            TomletMain.RegisterMapper<IControlMapping>(
                    null,
                    tomlValue =>
                    {
                        if (!(tomlValue is Tomlet.Models.TomlTable tomlTable))
                            throw new Tomlet.Exceptions.TomlTypeMismatchException(typeof(Tomlet.Models.TomlTable), tomlValue.GetType(), typeof(IControlMapping));

                        var typeName = tomlTable.GetString("TypeName");
                        if (typeName == null) throw new Exception("Error loading control mapping: TypeName not given!");

                        var cls = allowableLoadedTypes.FirstOrDefault(cls => cls.Name == typeName);
                        if (cls == null) throw new Exception($"Error loading control mapping: Not allowed to parse a {typeName}");

                        // Use reflection to call the generic method using the type we found before 
                        // Have fun when this inevitably breaks
                        return (IControlMapping)typeof(TomletMain)
                            .GetMethod("To", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                            null, System.Reflection.CallingConventions.Any,
                            new Type[] { typeof(Tomlet.Models.TomlValue) }, null)
                            .MakeGenericMethod(cls)
                            .Invoke(null, parameters: new object[] { tomlTable });
                    }
            );

            // Tomlet fails to save and load chars for some reason, so fix that
            TomletMain.RegisterMapper<char>(
                ch => new Tomlet.Models.TomlString(ch.ToString()),
                tomlValue =>
                {
                    if (!(tomlValue is Tomlet.Models.TomlString tomlString))
                        throw new Tomlet.Exceptions.TomlTypeMismatchException(typeof(Tomlet.Models.TomlString), tomlValue.GetType(), typeof(char));
                    return tomlString.StringValue.Length > 0 ? tomlString.StringValue[0] : '\0';
                }
            );
        }

        // Types we are allowed to load through the reflection-based stuff above.
        // Should prevent any security holes related to potentially instancing an arbitrary type
        private static List<Type> allowableLoadedTypes = new()
        {
            typeof(AxisControlMapping),
            typeof(ButtonControlMapping),
            typeof(SimpleKeyboardControlMapping),
            typeof(ThreePosKeyboardControlMapping),
        };
    }

    public class AxisControlMapping : IControlMapping
    {
        public int Axis { get; set; }
        public float Multiplier { get; set; } = 1;
        public bool Inverted { get; set; } = false;
        public float DeadzoneEnd { get; set; } = 0.025f;
        public float DeadzoneRest { get; set; } = 0.025f;

        public override float? ProcessEvent(InputEvent _event)
        {
            if (_event is InputEventJoypadMotion motionEvent && motionEvent.Axis == Axis)
            {
                // Apply the mapping to the raw value
                // Inputs and outputs are in the -1 to 1 range

                float value = motionEvent.AxisValue;
                value = Mathf.Clamp(value * Multiplier, -1, 1);

                if (Inverted) value = -value;

                if (value < -1 + DeadzoneEnd) value = -1;
                if (value > 1 - DeadzoneEnd) value = 1;
                if (Mathf.Abs(value) < DeadzoneRest) value = 0;

                return value;
            }
            return null;
        }
    }

    public class ButtonControlMapping : IControlMapping
    {
        public int ButtonIndex { get; set; }
        public bool Inverted { get; set; }
        public bool Momentary { get; set; }
        private float currentValue = -1;

        public override float? ProcessEvent(InputEvent _event)
        {
            if (_event is InputEventJoypadButton buttonEvent && buttonEvent.ButtonIndex == ButtonIndex)
            {
                // Apply the mapping to the raw value
                // Inputs and outputs are in the -1 to 1 range

                if (Momentary)
                {
                    float value = buttonEvent.Pressed ? 1 : -1;
                    if (Inverted) value = -value;
                    return value;
                }
                else
                {
                    if (buttonEvent.Pressed)
                    {
                        currentValue = -currentValue;
                        return currentValue;
                    }
                }
            }
            return null;
        }
    }

    public class SimpleKeyboardControlMapping : IControlMapping
    {
        public uint KeyScancode { get; set; } = 65;
        public bool Momentary { get; set; }
        private float currentValue = -1;

        public override float? ProcessEvent(InputEvent _event)
        {
            if (_event is InputEventKey keyEvent && keyEvent.Scancode == KeyScancode && !keyEvent.Echo)
            {
                if (Momentary) return keyEvent.Pressed ? 1 : -1;
                else
                {
                    if (keyEvent.Pressed)
                    {
                        currentValue = -currentValue;
                        return currentValue;
                    }
                }
            }
            return null;
        }
    }

    public class ThreePosKeyboardControlMapping : IControlMapping
    {
        // Useful for flaps
        public uint Key1Scancode { get; set; }
        public uint Key2Scancode { get; set; }
        public uint Key3Scancode { get; set; }


        public override float? ProcessEvent(InputEvent _event)
        {
            if (_event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
            {
                if (keyEvent.Scancode == Key1Scancode) return -1;
                if (keyEvent.Scancode == Key2Scancode) return 0;
                if (keyEvent.Scancode == Key3Scancode) return 1;
            }

            return null;
        }
    }
}
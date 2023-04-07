using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Tomlet;

namespace SimSettings
{
    public class Settings
    {
        public SimInput.InputMap InputMap { get; set; } = new();
        public List<SimInput.IControlMapping> ControlMappings { get; set; } = new()
        {
            new SimInput.AxisControlMapping() { ChannelName = "throttle", Axis = 0 },
            new SimInput.AxisControlMapping() { ChannelName = "aileron", Axis = 2 },
            new SimInput.AxisControlMapping() { ChannelName = "elevator", Axis = 3 },
            new SimInput.AxisControlMapping() { ChannelName = "rudder", Axis = 1 },
        };
        public Locations.GroundCamera.ZoomSettings GroundCameraZoom { get; set; } = new();
        public GraphicsSettings Graphics { get; set; } = new();
        public MiscSettings Misc { get; set; } = new();

        public void Apply()
        {
            // Some settings need to run code in order to apply, so do it here

            Engine.IterationsPerSecond = Misc.PhysicsFps;

            // We currently don't have a UI for setting this so just copy them from the old ones set by UI for now
            ControlMappings = InputMap.AxisMappings.Select(m => (SimInput.IControlMapping)new SimInput.AxisControlMapping()
            {
                ChannelName = m.Name,
                Axis = m.Axis,
                Inverted = m.Inverted,
                DeadzoneRest = m.DeadzoneRest,
                DeadzoneEnd = m.DeadzoneEnd,
                Multiplier = m.Multiplier
            }).ToList();
            ControlMappings.Add(new SimInput.ThreePosKeyboardControlMapping()
            {
                ChannelName = "flaps",
                Key1Scancode = (uint)KeyList.A,
                Key2Scancode = (uint)KeyList.S,
                Key3Scancode = (uint)KeyList.D,
            });
        }

        #region StaticSection

        public static readonly string SavePath = "user://settings.toml";
        public static Settings Current { get; private set; } = new();

        public static void SetCurrent(Settings settings)
        {
            Current = Clone(settings);
            Current.Apply();
        }

        public static Settings Load()
        {
            using var f = new File();
            if (f.Open(SavePath, File.ModeFlags.Read) == Error.Ok)
            {
                return TomletMain.To<Settings>(f.GetAsText());
            }
            else
            {
                return new Settings();
            }
        }
        public static void LoadCurrent()
        {
            SetCurrent(Load());
        }

        public static void Save(Settings settings)
        {
            using var f = new File();
            f.Open(SavePath, File.ModeFlags.Write);
            f.StoreString(TomletMain.TomlStringFrom(settings));
        }

        public static void SaveCurrent()
        {
            Save(Current);
        }

        public static Settings Clone(Settings settings)
        {
            // Some might consider this hacky, I think it's not since we already have ensured that the TOML conversion is safe
            return TomletMain.To<Settings>(TomletMain.TomlStringFrom(settings));
        }

        public static Settings CloneCurrent()
        {
            return Clone(Current);
        }

        #endregion StaticSection
    }
}
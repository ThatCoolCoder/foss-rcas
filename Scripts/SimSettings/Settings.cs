using Godot;
using System;
using Tomlet;

namespace SimSettings
{
    public class Settings
    {

        // public SimInputMap InputMap todo: code that class
        public GroundCameraZoomSettings GroundCameraZoomSettings { get; set; } = new();
        public string LastLoadedAircraft { get; set; } = null;
        public string LastLoadedLocation { get; set; } = null;
        public int PhysicsFps { get; set; } = 1000; // might as well make this configurable since 1000 might be a bit much on slow computers

        #region StaticSection

        public static readonly string SavePath = "user://settings.toml";
        public static Settings Current { get; private set; } = null;

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
        public static void LoadCurrentSettings()
        {
            Current = Load();
        }

        public static void Save(Settings settings)
        {
            using var f = new File();
            f.Open(SavePath, File.ModeFlags.Write);
            f.StoreString(TomletMain.TomlStringFrom(settings));
        }

        public static void SaveCurrentSettings()
        {
            Save(Current);
        }

        #endregion StaticSection
    }
}
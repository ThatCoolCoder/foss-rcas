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
        public string AddOnRepositoryPath { get; set; } = "user://AddOnContent/";

        public void Apply()
        {
            // Some settings need to run code in order to apply, so do it here

            Engine.IterationsPerSecond = PhysicsFps;
        }

        #region StaticSection

        public static readonly string SavePath = "user://settings.toml";
        public static Settings Current { get; private set; } = null;

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
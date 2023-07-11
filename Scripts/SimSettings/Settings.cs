using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Tomlet;

namespace SimSettings;

public partial class Settings
{
    public Locations.GroundCamera.ZoomSettings GroundCameraZoom { get; set; } = new();
    public GraphicsSettings Graphics { get; set; } = new();
    public MiscSettings Misc { get; set; } = new();
    // At the end so that manual editing of the settings file isn't cluttered by this
    public SimInput.InputMap InputMap { get; set; } = SimInput.InputMap.DefaultMap;

    public void Apply()
    {
        // Some settings need to run code in order to apply, so do it here

        Misc.Apply();
        Graphics.Apply();
    }

    public void ApplyToViewport(SubViewport viewport)
    {
        // Some settings need to run code in a viewport, so do it here

        Graphics.ApplyToViewport(viewport);
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
        using var f = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (f != null)
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
        using var f = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
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
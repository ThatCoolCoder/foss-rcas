using Godot;
using System;

namespace UI.Settings.Components;

public delegate T SettingReader<T>(SimSettings.Settings settings);
public delegate void SettingWriter<T>(SimSettings.Settings settings, T value);
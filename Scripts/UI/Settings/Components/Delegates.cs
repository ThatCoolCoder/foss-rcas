using Godot;
using System;

namespace UI.Settings.Components
{
    public delegate T SettingReader<T>();
    public delegate void SettingWriter<T>(T value);
}
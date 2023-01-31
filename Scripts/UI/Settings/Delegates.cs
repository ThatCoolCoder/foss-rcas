using Godot;
using System;

namespace UI.Settings
{
    delegate T SettingReader<T>();
    delegate void SettingWriter<T>(T value);
}
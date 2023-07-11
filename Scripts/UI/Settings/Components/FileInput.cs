using Godot;
using System;

namespace UI.Settings.Components;

public partial class FileInput : SettingsRow<string>
{
    private LineEdit lineEdit;
    private FileDialog fileDialog;

    private FileDialog.FileModeEnum mode;

    public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/FileInput.tscn");

    public FileInput Config(Node parent, string name, SettingReader<string> read, SettingWriter<string> write,
        FileDialog.FileModeEnum _mode = FileDialog.FileModeEnum.OpenFile, string toolTip = "")
    {
        base.Config(parent, name, read, write, toolTip);


        lineEdit = GetNode<LineEdit>("HBoxContainer/LineEdit");
        fileDialog = GetNode<FileDialog>("HBoxContainer/FileDialog");

        fileDialog.FileMode = _mode;

        return this;
    }

    public override void OnSettingsChanged()
    {
        var value = read(SettingsScreen.NewSettings);
        if (value.StartsWith("res://") && !OS.HasFeature("editor"))
        {
            value = value.ReplaceFirst("res://", OS.GetExecutablePath().GetBaseDir());
        }
        else value = ProjectSettings.GlobalizePath(value);
        lineEdit.Text = value;
    }

    public void _on_SelectFileButton_pressed()
    {
        fileDialog.Popup();
    }

    public void _on_FileDialog_popup_hide()
    {
        write(SettingsScreen.NewSettings, fileDialog.CurrentPath);
        OnSettingsChanged();
    }

}

using Godot;
using System;

namespace UI.Settings.Components
{
    public class FileInput : SettingsRow<string>
    {
        private LineEdit lineEdit;
        private FileDialog fileDialog;

        private FileDialog.ModeEnum mode;

        public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/Components/FileInput.tscn");

        public FileInput Config(Node parent, string name, SettingReader<string> read, SettingWriter<string> write,
            FileDialog.ModeEnum _mode = FileDialog.ModeEnum.OpenFile, string toolTip = "")
        {
            base.Config(parent, name, read, write, toolTip);


            lineEdit = GetNode<LineEdit>("HBoxContainer/LineEdit");
            fileDialog = GetNode<FileDialog>("HBoxContainer/FileDialog");

            fileDialog.Mode = _mode;

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
            fileDialog.Popup_();
        }

        public void _on_FileDialog_popup_hide()
        {
            write(SettingsScreen.NewSettings, fileDialog.CurrentPath);
            OnSettingsChanged();
        }

    }
}

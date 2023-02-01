using Godot;
using System;

namespace UI.Settings
{
    public class SettingsRow<T> : HSplitContainer
    {
        // todo: code me and add children

        private string name;
        private SettingReaderT> reader;
        private SettingWriterT> writer;

        public SettingsRow(string _name, SettingReader<T> _reader, SettingReader<T> _writer)
        {
            reader = _reader;
            writer = _writer;
            name = _name;
        }

        public override void _Ready()
        {
            GetNode<Label>("Label").Text = name;
        } 
    }
}
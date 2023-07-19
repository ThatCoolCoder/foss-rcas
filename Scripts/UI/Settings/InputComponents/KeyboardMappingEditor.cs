using Godot;
using System;

namespace UI.Settings.InputComponents;

using Components;

public partial class KeyboardMappingEditor : BaseControlMappingEditor
{
	// It was decided to not make this a smart control using readers and writers, and simply regenerate them every time the settings change.
	// The performance should still be fine and this makes it SO much easier to code.

	private SimInput.KeyboardControlMapping controlMapping;
	private KeyInput key2Input;
	private KeyInput key3Input;

	public static PackedScene Scene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Settings/InputComponents/KeyboardMappingEditor.tscn");

	public KeyboardMappingEditor Config(Node parent, SimInput.KeyboardControlMapping _controlMapping)
	{
		controlMapping = _controlMapping;
		if (parent != null) parent.AddChild(this);

		return this;
	}

	public override void _Ready()
	{
		var holder = GetMainItemHolder();

		holder.GetNode<KeyInput>("KeyInput").Config(null, "Key",
			s => (Key)controlMapping.KeyScancode,
			(s, v) => controlMapping.KeyScancode = (uint)v).OnSettingsChanged();

		// holder.GetNode<BooleanInput>("MomentaryInput").Config(null, "Momentary",
		//     s => controlMapping.Momentary,
		//     (s, v) => controlMapping.Momentary = v,
		//     toolTip: "Whether the input is momentary or toggles").OnSettingsChanged();

		holder.GetNode<EnumInput>("TypeInput").Config(null, "Mode",
			s => controlMapping.MappingType,
			(s, v) => {
				controlMapping.MappingType = (SimInput.KeyboardControlMapping.MappingTypeEnum) v;
				AdjustInputVisibilities();
			},
			enumType: typeof(SimInput.KeyboardControlMapping.MappingTypeEnum));
		

		key2Input = holder.GetNode<KeyInput>("Key2Input");
		key2Input.Config(null, "Key 2",
			s => (Key)controlMapping.Key2Scancode,
			(s, v) => controlMapping.Key2Scancode = (uint)v).OnSettingsChanged();

		key3Input = holder.GetNode<KeyInput>("Key3Input");
		key3Input.Config(null, "Key 3",
			s => (Key)controlMapping.Key3Scancode,
			(s, v) => controlMapping.Key3Scancode = (uint)v).OnSettingsChanged();
		
		AdjustInputVisibilities();
	}

	private void AdjustInputVisibilities()
	{
		var visible = controlMapping.MappingType == SimInput.KeyboardControlMapping.MappingTypeEnum.ThreePosition;
		key2Input.Visible = visible;
		key3Input.Visible = visible;
	}
}

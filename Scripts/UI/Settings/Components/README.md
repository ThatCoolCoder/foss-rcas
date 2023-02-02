# Components for the settings editor

To make it easier to add plain settings to the editor, I've created some classes & scenes extending from SettingsRow. These link directly to a property. They can't be used for complex things like dependent properties - to achieve such things sub-editors must manually be programmed.

Yes, currently this is overkill. However, it makes it very easy to expand to a far greater number of settings in future, which is something I envisage happening.
# Components for the settings editor

To make it easier to add plain settings to the editor, I've created some classes & scenes that can be quickly instanced from code.

Most of them directly inherit from SettingsRow. These link directly to a property. They can't be used for complex things like dependent properties - to achieve such things sub-editors must manually be programmed.

Some manually-coded scenes have also been created. You can tell the SettingsRow-derived classes apart from the rest as they all end in `Input`.

There are also a bunch of similar classes within the UI directory that use this same principle without extending from SettingsRow.

Yes, currently this is overkill. However, it makes it very easy to expand to a far greater number of settings in future, which is something I envisage happening.
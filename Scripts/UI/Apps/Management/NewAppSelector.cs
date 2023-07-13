using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Apps.Management;

public partial class NewAppSelector : Popup
{
    [Export] public UIAppInfoList AvailableApps { get; set; }
    [Export] private OptionButton appSelector;
    [Export] private Label description;
    private List<UIAppInfo> appList;
    private UIAppInfo candidateApp;
    public UIAppInfo SelectedApp { get; private set; }

    public override void _Ready()
    {
        appList = AvailableApps.Apps.OrderBy(x => x.Name).ToList();
        appSelector.Clear();
        foreach (var app in appList)
        {
            appSelector.AddItem(app.Name);
        }
        _on_AppSelector_item_selected(0);
    }

    private void _on_about_to_popup()
    {
        SelectedApp = null;
    }

    private void _on_AppSelector_item_selected(int index)
    {
        candidateApp = appList[index];
        description.Text = candidateApp.Description;
    }

    private void _on_Cancel_pressed()
    {
        Hide();
    }

    private void _on_Add_pressed()
    {
        SelectedApp = candidateApp;
        Hide();
    }
}
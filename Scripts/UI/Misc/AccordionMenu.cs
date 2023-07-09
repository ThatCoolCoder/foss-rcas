using Godot;
using System;

namespace UI.Misc
{
    [Tool]
    public partial class AccordionMenu : VBoxContainer
    {
        // Collection of collapsible menus in which only one can be open at a time.
        // They start all closed


        public override void _Ready()
        {
            CollapseAll();
        }

        private void _on_AccordionMenu_child_entered_tree(Node child)
        {
            if (child is CollapsibleMenu menu)
            {
                menu.IsOpen = false;
                menu.OnToggleOpen += OnChildStateChanged;
            }
        }

        private void _on_AccordionMenu_child_exiting_tree(Node child)
        {
            if (child is CollapsibleMenu menu)
            {
                menu.IsOpen = false;
                menu.OnToggleOpen -= OnChildStateChanged;
            }
        }

        private void OnChildStateChanged(CollapsibleMenu changedMenu, bool isOpen)
        {
            if (isOpen)
            {
                foreach (var child in this.GetChildNodeList())
                {
                    if (child is CollapsibleMenu menu && changedMenu != menu) menu.IsOpen = false;
                }
            }
        }

        private void CollapseAll()
        {
            foreach (var child in this.GetChildNodeList())
            {
                if (child is CollapsibleMenu menu) menu.IsOpen = false;
            }
        }
    }
}
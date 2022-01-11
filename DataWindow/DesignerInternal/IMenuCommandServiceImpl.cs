using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DataWindow.DesignerInternal.Event;

namespace DataWindow.DesignerInternal
{
    internal class IMenuCommandServiceImpl : IMenuCommandService
    {
        private readonly IDictionary commands;

        private readonly IDictionary globalVerbs;

        private readonly IDesignerHost host;

        private readonly ContextMenu menu;

        private readonly IDictionary menuItemVerb;

        public IMenuCommandServiceImpl(IDesignerHost host)
        {
            this.host = host;
            commands = new Hashtable();
            globalVerbs = new Hashtable();
            menuItemVerb = new Hashtable();
            menu = new ContextMenu();
        }

        public DesignerVerbCollection Verbs
        {
            get
            {
                var designerVerbCollection = new DesignerVerbCollection();
                foreach (var obj in ((ISelectionService) host.GetService(typeof(ISelectionService))).GetSelectedComponents())
                {
                    var component = (IComponent) obj;
                    var designer = host.GetDesigner(component);
                    if ((designer != null ? designer.Verbs : null) != null)
                        foreach (var obj2 in designer.Verbs)
                        {
                            var designerVerb = (DesignerVerb) obj2;
                            if (AddingVerb == null || AddingVerb(component, designerVerb)) designerVerbCollection.Add(designerVerb);
                        }
                }

                foreach (var obj3 in globalVerbs.Values)
                {
                    var value = (DesignerVerb) obj3;
                    designerVerbCollection.Add(value);
                }

                return designerVerbCollection;
            }
        }

        public void AddCommand(MenuCommand command)
        {
            if (command == null) throw new NullReferenceException("command");
            if (FindCommand(command.CommandID) == null)
            {
                commands.Add(command.CommandID, command);
                return;
            }

            throw new InvalidOperationException("adding existing command");
        }

        public void RemoveCommand(MenuCommand command)
        {
            if (command != null) commands.Remove(command.CommandID);
        }

        public MenuCommand FindCommand(CommandID commandID)
        {
            return (MenuCommand) commands[commandID];
        }

        public bool GlobalInvoke(CommandID commandID)
        {
            var designerVerb = globalVerbs[commandID] as DesignerVerb;
            if (designerVerb != null)
            {
                designerVerb.Invoke();
                return true;
            }

            var menuCommand = FindCommand(commandID);
            if (menuCommand != null)
            {
                menuCommand.Invoke();
                return true;
            }

            return false;
        }

        public void AddVerb(DesignerVerb verb)
        {
            if (verb == null) throw new NullReferenceException("verb");
            globalVerbs.Add(verb.CommandID, verb);
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            if (verb == null) throw new NullReferenceException("verb");
            globalVerbs.Remove(verb.CommandID);
        }

        public void ShowContextMenu(CommandID menuID, int x, int y)
        {
            var verbs = Verbs;
            var num = verbs.Count - globalVerbs.Values.Count;
            var num2 = 0;
            menu.MenuItems.Clear();
            menuItemVerb.Clear();
            foreach (var obj in verbs)
            {
                var designerVerb = (DesignerVerb) obj;
                if (designerVerb.Visible)
                {
                    if (num > 0 && num2 == num) menu.MenuItems.Add(new MenuItem("-"));
                    var menuItem = new MenuItem(designerVerb.Text);
                    menuItem.Click += MenuItemClickHandler;
                    menuItemVerb.Add(menuItem, designerVerb);
                    menuItem.Enabled = designerVerb.Enabled;
                    menuItem.Checked = designerVerb.Checked;
                    menu.MenuItems.Add(menuItem);
                    num2++;
                }
            }

            var control = ((ISelectionService) host.GetService(typeof(ISelectionService))).PrimarySelection as Control;
            var control2 = control != null ? control : (Control) host.RootComponent;
            var point = control2.PointToScreen(new Point(0, 0));
            menu.Show(control2, new Point(x - point.X, y - point.Y));
        }

        internal event AddingVerbHandler AddingVerb;

        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            MenuItem key;
            DesignerVerb designerVerb;
            if ((key = sender as MenuItem) != null && (designerVerb = menuItemVerb[key] as DesignerVerb) != null)
                try
                {
                    designerVerb.Invoke();
                }
                catch
                {
                }
        }
    }
}
using System.ComponentModel.Design;

namespace DataWindow.DesignerInternal.Event
{
    internal class RemoveAction : AddAction
    {
        public RemoveAction(IDesignerHost host, object item, MegaAction owner) : base(host, item, owner)
        {
            properties = owner.StoreProperties(item, null, null);
        }

        public override void Undo()
        {
            base.Redo();
        }

        public override void Redo()
        {
            base.Undo();
        }
    }
}
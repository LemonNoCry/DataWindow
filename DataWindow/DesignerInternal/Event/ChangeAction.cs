using System.Collections;
using System.ComponentModel.Design;

namespace DataWindow.DesignerInternal.Event
{
    internal class ChangeAction : DesignerAction
    {
        private readonly Hashtable _newValue;

        private readonly Hashtable _oldValue;

        public ChangeAction(IDesignerHost host, object control, Hashtable oldValues, Hashtable newValues, MegaAction owner) : base(host, control, owner)
        {
            _oldValue = new Hashtable();
            _newValue = new Hashtable();
            foreach (var obj in oldValues)
            {
                var dictionaryEntry = (DictionaryEntry) obj;
                var obj2 = newValues[dictionaryEntry.Key];
                if (obj2 != null)
                {
                    if (!obj2.Equals(dictionaryEntry.Value))
                    {
                        _oldValue.Add(dictionaryEntry.Key, dictionaryEntry.Value);
                        _newValue.Add(dictionaryEntry.Key, obj2);
                    }

                    newValues.Remove(dictionaryEntry.Key);
                }
                else
                {
                    _oldValue.Add(dictionaryEntry.Key, dictionaryEntry.Value);
                }
            }

            foreach (var obj3 in newValues)
            {
                var dictionaryEntry2 = (DictionaryEntry) obj3;
                _newValue.Add(dictionaryEntry2.Key, dictionaryEntry2.Value);
            }
        }

        public override void Undo()
        {
            ((ISelectionService) host.GetService(typeof(ISelectionService))).SetSelectedComponents(null);
            SetProperties(_oldValue);
        }

        public override void Redo()
        {
            ((ISelectionService) host.GetService(typeof(ISelectionService))).SetSelectedComponents(null);
            SetProperties(_newValue);
        }
    }
}
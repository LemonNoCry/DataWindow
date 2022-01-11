using System.ComponentModel;

namespace DataWindow.DesignerInternal.Interface
{
    public interface IFormComponents
    {
        IComponent Get(string name);

        void Add(string name, IComponent component);

        void Remove(string name);
    }
}
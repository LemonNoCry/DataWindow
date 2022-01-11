using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace DataWindow.DesignerInternal
{
    internal class INameCreationServiceImpl : INameCreationService
    {
        public string CreateName(IContainer container, Type dataType)
        {
            var num = 0;
            var name = dataType.Name;
            string text;
            do
            {
                num++;
                text = name + num;
            } while ((container != null ? container.Components[text] : null) != null);

            return text;
        }

        public bool IsValidName(string name)
        {
            if (name == null || name.Length == 0) return false;
            if (!char.IsLetter(name, 0)) return false;
            for (var i = 0; i < name.Length; i++)
                if (!char.IsLetterOrDigit(name, i) && name[i] != '_' && name[i] != ' ' && name[i] != '-' && name[i] != '.')
                    return false;
            return true;
        }

        public void ValidateName(string name)
        {
            if (!IsValidName(name)) throw new ArgumentException("无效的名称: " + name);
        }
    }
}
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Windows.Forms;

namespace DataWindow.Serialization.Components
{
    internal class ComponentSerializationServiceImpl : ComponentSerializationService
    {
        private readonly DefaultDesignerLoader _designerLoader;

        internal ComponentSerializationServiceImpl(DefaultDesignerLoader designerLoader)
        {
            _designerLoader = designerLoader;
        }

        public override SerializationStore CreateStore()
        {
            return new SerializationStoreImpl();
        }

        public override SerializationStore LoadStore(Stream stream)
        {
            return new SerializationStoreImpl(stream);
        }

        public override ICollection Deserialize(SerializationStore store)
        {
            return Deserialize(store, null);
        }

        public override ICollection Deserialize(SerializationStore _store, IContainer container)
        {
            var serializationStoreImpl = _store as SerializationStoreImpl;
            if (serializationStoreImpl == null) return null;
            var collection = _designerLoader.Deserialize(serializationStoreImpl.Reader, false);
            if (container != null)
            {
                var enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IComponent component;
                    if ((component = enumerator.Current as IComponent) != null) container.Add(component);
                }
            }

            return collection;
        }

        public override void DeserializeTo(SerializationStore _store, IContainer container, bool validateRecycledTypes, bool applyDefaults)
        {
            SerializationStoreImpl serializationStoreImpl;
            if ((serializationStoreImpl = _store as SerializationStoreImpl) != null)
            {
                var loadMode = _designerLoader.LoadMode;
                if (validateRecycledTypes) _designerLoader.LoadMode = LoadModes.ModifyExisting;
                _designerLoader.Load(_designerLoader.DesignerHost.RootComponent as Control, serializationStoreImpl.Reader, null, true);
                _designerLoader.LoadMode = loadMode;
            }
        }

        public override void Serialize(SerializationStore _store, object value)
        {
            SerializationStoreImpl serializationStoreImpl;
            if ((serializationStoreImpl = _store as SerializationStoreImpl) != null)
            {
                _designerLoader.BeforeWriting();
                _designerLoader.StoreControl(value, null, serializationStoreImpl.Writer);
                _designerLoader.AfterWriting();
            }
        }

        public override void SerializeAbsolute(SerializationStore _store, object value)
        {
            SerializationStoreImpl serializationStoreImpl;
            if ((serializationStoreImpl = _store as SerializationStoreImpl) != null)
            {
                var storeMode = _designerLoader.StoreMode;
                _designerLoader.StoreMode = StoreModes.AllProperties;
                _designerLoader.BeforeWriting();
                _designerLoader.StoreControl(value, null, serializationStoreImpl.Writer);
                _designerLoader.AfterWriting();
                _designerLoader.StoreMode = storeMode;
            }
        }

        public override void SerializeMember(SerializationStore _store, object owningObject, MemberDescriptor member)
        {
            SerializationStoreImpl serializationStoreImpl;
            PropertyDescriptor propertyDescriptor;
            if ((serializationStoreImpl = _store as SerializationStoreImpl) != null && (propertyDescriptor = member as PropertyDescriptor) != null && propertyDescriptor.ShouldSerializeValue(owningObject)) _designerLoader.StoreMember(owningObject, propertyDescriptor, _designerLoader.DesignerHost.RootComponent as Control, serializationStoreImpl.Writer);
        }

        public override void SerializeMemberAbsolute(SerializationStore _store, object owningObject, MemberDescriptor member)
        {
            SerializationStoreImpl serializationStoreImpl;
            PropertyDescriptor prop;
            if ((serializationStoreImpl = _store as SerializationStoreImpl) != null && (prop = member as PropertyDescriptor) != null) _designerLoader.StoreMember(owningObject, prop, _designerLoader.DesignerHost.RootComponent as Control, serializationStoreImpl.Writer);
        }
    }
}
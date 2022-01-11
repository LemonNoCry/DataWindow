using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.IO;

namespace DataWindow.Serialization.Components
{
    internal class SerializationStoreImpl : SerializationStore
    {
        private readonly MemoryStream stream;

        internal SerializationStoreImpl()
        {
            stream = new MemoryStream();
            Writer = new TextFormWriter(stream);
            Reader = new TextFormReader(stream);
        }

        internal SerializationStoreImpl(Stream stream)
        {
            Reader = new TextFormReader(stream);
            Writer = new TextFormWriter(stream);
        }

        internal bool Closed { get; private set; }

        internal IReader Reader { get; }

        internal IWriter Writer { get; }

        public override ICollection Errors => new string[0];

        public override void Close()
        {
            if (!Closed)
            {
                var writer = Writer;
                if (writer != null) writer.Flush();
                Closed = true;
            }

            stream.Seek(0L, SeekOrigin.Begin);
        }

        public override void Save(Stream destStream)
        {
            Close();
            stream.WriteTo(destStream);
        }
    }
}
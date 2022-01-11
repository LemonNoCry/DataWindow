using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace DataWindow.Serialization.Components
{
    public class XmlFormWriter : IWriter, IDisposable, ILazyWrite
    {
        private readonly XmlWriter writer;
        private int callCount;

        private XmlWriter curWriter;

        private XmlWriter lazyWriter;

        private StringWriter stringWriter;

        public XmlFormWriter(string fileName)
        {
            writer = new XmlTextWriter(fileName, Encoding.UTF8);
            curWriter = writer;
        }

        public XmlFormWriter(XmlWriter stream)
        {
            writer = stream;
            curWriter = writer;
        }

        public XmlFormWriter(Stream stream)
        {
            writer = new XmlTextWriter(stream, Encoding.UTF8);
            curWriter = writer;
        }

        public void Begin()
        {
            if (callCount == 0)
            {
                stringWriter = new StringWriter();
                lazyWriter = new XmlTextWriter(stringWriter);
                curWriter = lazyWriter;
            }

            callCount++;
        }

        public void End(bool cancel)
        {
            callCount--;
            if (callCount == 0)
            {
                curWriter = writer;
                if (lazyWriter != null)
                {
                    if (!cancel)
                    {
                        lazyWriter.Flush();
                        var text = stringWriter.ToString();
                        if (text.Length > 0)
                        {
                            var reader = new XmlTextReader(new StringReader(text));
                            writer.WriteNode(reader, false);
                        }
                    }

                    stringWriter = null;
                    lazyWriter = null;
                }
            }
        }

        public virtual void WriteStartElement(string name, Hashtable attributes)
        {
            curWriter.WriteStartElement(name);
            if (attributes != null)
                foreach (var obj in attributes)
                {
                    var dictionaryEntry = (DictionaryEntry) obj;
                    if (dictionaryEntry.Value != null) curWriter.WriteAttributeString(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
                }
        }

        public virtual void WriteEndElement(string name)
        {
            curWriter.WriteEndElement();
        }

        public virtual void WriteValue(string name, string value, Hashtable attributes)
        {
            WriteStartElement(name, attributes);
            curWriter.WriteString(value);
            curWriter.WriteEndElement();
        }

        public void Flush()
        {
            writer.Flush();
        }

        public void Dispose()
        {
            writer.Close();
            if (lazyWriter != null) lazyWriter.Close();
        }
    }
}
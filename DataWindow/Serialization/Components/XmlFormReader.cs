using System.IO;
using System.Xml;

namespace DataWindow.Serialization.Components
{
    public class XmlFormReader : ReaderBase<XmlReader>
    {
        private bool isEmptyValue;

        public XmlFormReader(string fileName)
        {
            reader = new XmlTextReader(fileName);
        }

        public XmlFormReader(XmlReader stream)
        {
            reader = stream;
        }

        public XmlFormReader(Stream stream)
        {
            reader = new XmlTextReader(stream);
        }

        public override bool Read()
        {
            if (State == ReaderState.Error || State == ReaderState.EOF || reader.ReadState != ReadState.Initial && reader.ReadState != ReadState.Interactive) return false;
            if (State == ReaderState.Initial && !ReadUntil(XmlNodeType.Element))
            {
                State = ReaderState.Error;
                return false;
            }

            if (isEmptyValue)
            {
                isEmptyValue = false;
                ReadNext();
            }

            if (reader.NodeType == XmlNodeType.Element)
            {
                State = ReaderState.StartElement;
                Name = reader.Name;
                isEmptyValue = reader.IsEmptyElement;
                ReadAttributes();
                if (isEmptyValue)
                {
                    Value = "";
                    State = ReaderState.Value;
                    return true;
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                State = ReaderState.EndElement;
                Name = "";
            }
            else
            {
                State = ReaderState.Value;
            }

            if (!ReadNext())
            {
                State = reader.ReadState == ReadState.EndOfFile ? ReaderState.EOF : ReaderState.Error;
                return false;
            }

            if (reader.NodeType == XmlNodeType.Text)
            {
                Value = reader.Value;
                if (!ReadNext() || reader.NodeType != XmlNodeType.EndElement)
                {
                    State = ReaderState.Error;
                    return false;
                }

                State = ReaderState.Value;
                ReadNext();
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == Name)
            {
                Value = "";
                State = ReaderState.Value;
                ReadNext();
            }

            return true;
        }

        private void ReadAttributes()
        {
            Attributes.Clear();
            while (reader.MoveToNextAttribute()) Attributes[reader.Name] = reader.Value;
        }

        private bool ReadUntil(XmlNodeType nodeType)
        {
            while (ReadNext())
                if (reader.NodeType == nodeType)
                    return true;
            return false;
        }

        private bool ReadNext()
        {
            while (reader.Read())
            {
                var nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.Text || nodeType == XmlNodeType.EndElement) return true;
            }

            return false;
        }

        public override void Dispose()
        {
            reader.Close();
        }
    }
}
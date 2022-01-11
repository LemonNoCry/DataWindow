using System;
using System.Collections;
using System.IO;

namespace DataWindow.Serialization.Components
{
    public class TextFormWriter : IWriter, IDisposable, ILazyWrite
    {
        private readonly TextWriter writer;
        private int callCount;

        private string currentIndent = "";

        private TextWriter curWriter;

        private StringWriter lazyWriter;

        public TextFormWriter(string fileName)
        {
            writer = new StreamWriter(fileName);
            curWriter = writer;
        }

        public TextFormWriter(TextWriter stream)
        {
            writer = stream;
            curWriter = writer;
        }

        public TextFormWriter(Stream stream)
        {
            writer = new StreamWriter(stream);
            curWriter = writer;
        }

        public int Indentation { get; set; } = 3;

        public void Begin()
        {
            if (callCount == 0)
            {
                lazyWriter = new StringWriter();
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
                        writer.Write(lazyWriter.ToString());
                    }

                    lazyWriter = null;
                }
            }
        }

        public void WriteStartElement(string name, Hashtable attributes)
        {
            curWriter.Write(currentIndent);
            curWriter.Write("Begin ");
            curWriter.Write(name);
            curWriter.Write(" ");
            WriteAttributes(attributes);
            curWriter.WriteLine();
            for (var i = 0; i < Indentation; i++) currentIndent += " ";
        }

        public void WriteValue(string name, string value, Hashtable attributes)
        {
            curWriter.Write(currentIndent);
            curWriter.Write("{0}", name);
            WriteAttributes(attributes);
            value = value.Replace(Environment.NewLine, "\\n");
            curWriter.WriteLine("={0}", value);
        }

        public void WriteEndElement(string name)
        {
            currentIndent = currentIndent.Remove(currentIndent.Length - Indentation, Indentation);
            curWriter.Write(currentIndent);
            curWriter.WriteLine("End");
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

        private void WriteAttributes(Hashtable attributes)
        {
            if (attributes != null && attributes.Count != 0)
            {
                var flag = true;
                curWriter.Write("[");
                foreach (var obj in attributes)
                {
                    var dictionaryEntry = (DictionaryEntry) obj;
                    if (dictionaryEntry.Value != null)
                    {
                        if (flag)
                            flag = false;
                        else
                            curWriter.Write(" ");
                        curWriter.Write("{0}=\"{1}\"", dictionaryEntry.Key, dictionaryEntry.Value);
                    }
                }

                curWriter.Write("]");
            }
        }
    }
}
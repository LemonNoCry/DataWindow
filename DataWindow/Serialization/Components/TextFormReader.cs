using System;
using System.IO;

namespace DataWindow.Serialization.Components
{
    public class TextFormReader : ReaderBase<TextReader>
    {
        private string curLine;

        public TextFormReader(string fileName)
        {
            reader = new StreamReader(fileName);
        }

        public TextFormReader(TextReader stream)
        {
            reader = stream;
        }

        public TextFormReader(Stream stream)
        {
            reader = new StreamReader(stream);
        }

        public override bool Read()
        {
            if (State == ReaderState.EOF || !ReadNext()) return false;
            Attributes.Clear();
            if (curLine.IndexOf("Begin ") == 0)
            {
                var num = curLine.IndexOf('[');
                if (num == -1)
                {
                    var num2 = curLine.IndexOf('"');
                    var num3 = curLine.LastIndexOf('"');
                    Attributes["assembly"] = curLine.Substring(num2 + 1, num3 - num2 - 1);
                    Attributes["name"] = curLine.Substring(num3 + 1).Trim();
                    Name = "object";
                }
                else
                {
                    Name = curLine.Substring(6, num - 6).Trim();
                    SaveAttributes(curLine.Substring(num + 1, curLine.LastIndexOf(']') - num - 1));
                }

                State = ReaderState.StartElement;
            }
            else if (curLine.IndexOf("End") == 0)
            {
                State = ReaderState.EndElement;
            }
            else
            {
                State = ReaderState.Value;
                var num4 = curLine.IndexOf('[');
                int num6;
                if (num4 >= 0)
                {
                    var num5 = curLine.IndexOf(']', num4 + 1);
                    SaveAttributes(curLine.Substring(num4 + 1, num5 - num4 - 1));
                    Name = curLine.Substring(0, num4).Trim();
                    num6 = curLine.IndexOf('=', num5) + 1;
                }
                else
                {
                    num6 = curLine.IndexOf('=', 0) + 1;
                    if (num6 > 0) Name = curLine.Substring(0, num6 - 1).Trim();
                }

                Value = curLine.Substring(num6).Trim();
                Value = Value.Replace("\\n", Environment.NewLine);
            }

            return true;
        }

        private void SaveAttributes(string attributeString)
        {
            var num = 0;
            for (;;)
            {
                var num2 = attributeString.IndexOf('=', num);
                if (num2 < 0) break;
                var key = attributeString.Substring(num, num2 - num).Trim();
                var num3 = attributeString.IndexOf('"', num2);
                var num4 = attributeString.IndexOf('"', num3 + 1);
                if (num3 < 0 || num4 < 0) break;
                var value = attributeString.Substring(num3 + 1, num4 - num3 - 1).Trim();
                Attributes[key] = value;
                num = num4 + 1;
            }
        }

        private bool ReadNext()
        {
            if (State == ReaderState.Error || State == ReaderState.EOF) return false;
            try
            {
                char[] trimChars =
                {
                    ' '
                };
                for (;;)
                {
                    curLine = reader.ReadLine();
                    if (curLine == null) break;
                    curLine = curLine.TrimStart(trimChars);
                    if (curLine.Length != 0) goto Block_5;
                }

                State = ReaderState.EOF;
                return false;
                Block_5: ;
            }
            catch (Exception ex)
            {
                State = ex is EndOfStreamException ? ReaderState.EOF : ReaderState.Error;
                return false;
            }

            return true;
        }

        public override void Dispose()
        {
            reader.Close();
        }
    }
}
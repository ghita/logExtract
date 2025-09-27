using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace logExtract
{
    public class CSVExtract
    {
        private readonly Stream _stream;
        private readonly bool _hasHeader;
        private readonly Encoding _encoding;
        private List<string> _headers;

        public CSVExtract(Stream stream, bool hasHeader = true, Encoding encoding = null)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _hasHeader = hasHeader;
            _encoding = encoding ?? Encoding.UTF8;
        }

        public IEnumerable<Dictionary<string, string>> Parse()
        {
            using var reader = new StreamReader(_stream, _encoding, leaveOpen: true);
            string line;
            bool firstRow = true;
            StringBuilder fieldBuilder = new StringBuilder();
            List<string> fields = new List<string>();
            bool inQuotes = false;
            while ((line = reader.ReadLine()) != null)
            {
                int i = 0;
                while (i < line.Length)
                {
                    char c = line[i];
                    if (inQuotes)
                    {
                        if (c == '"')
                        {
                            if (i + 1 < line.Length && line[i + 1] == '"')
                            {
                                fieldBuilder.Append('"');
                                i += 2;
                                continue;
                            }
                            else
                            {
                                inQuotes = false;
                                i++;
                                continue;
                            }
                        }
                        else
                        {
                            fieldBuilder.Append(c);
                            i++;
                        }
                    }
                    else
                    {
                        if (c == ',')
                        {
                            fields.Add(fieldBuilder.ToString());
                            fieldBuilder.Clear();
                            i++;
                        }
                        else if (c == '"')
                        {
                            inQuotes = true;
                            i++;
                        }
                        else
                        {
                            fieldBuilder.Append(c);
                            i++;
                        }
                    }
                }
                if (inQuotes)
                {
                    fieldBuilder.Append("\r\n");
                    continue;
                }
                fields.Add(fieldBuilder.ToString());
                fieldBuilder.Clear();
                if (firstRow && _hasHeader)
                {
                    _headers = new List<string>(fields);
                    firstRow = false;
                }
                else
                {
                    var row = new Dictionary<string, string>();
                    for (int j = 0; j < fields.Count; j++)
                    {
                        string key = _hasHeader && _headers != null && j < _headers.Count ? _headers[j] : $"Column{j + 1}";
                        row[key] = fields[j];
                    }
                    yield return row;
                }
                fields.Clear();
            }
        }
    }
}

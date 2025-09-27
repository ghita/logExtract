using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using logExtract;
using Xunit;

namespace logExtract.Test
{
    public class CSVExtractTests
    {
        [Fact]
        public void Parse_SimpleCsv_NoMultiline_Works()
        {
            var path = Path.Combine("TestData", "simple.csv");
            using var stream = File.OpenRead(path);
            var parser = new CSVExtract(stream);
            var rows = parser.Parse().ToList();
            Assert.Equal(3, rows.Count);
            Assert.Equal("Alice", rows[0]["Name"]);
            Assert.Equal("Hello", rows[0]["Message"]);
            Assert.Equal("Bob", rows[1]["Name"]);
            Assert.Equal("World", rows[1]["Message"]);
            Assert.Equal("Charlie", rows[2]["Name"]);
            Assert.Equal("Test", rows[2]["Message"]);
        }

        [Fact]
        public void Parse_Csv_WithMultilineColumn_UnixLineEndings_Works()
        {
            var path = Path.Combine("TestData", "multiline-unix.csv");
            using var stream = File.OpenRead(path);
            var parser = new CSVExtract(stream);
            var rows = parser.Parse().ToList();
            Assert.Equal(3, rows.Count);
            Assert.Equal("Hello\r\nThis is a multiline\r\nmessage.", rows[0]["Message"]);
            Assert.Equal("Another\r\nmultiline\r\nentry", rows[1]["Message"]);
            Assert.Equal("Single line", rows[2]["Message"]);
        }

        [Fact]
        public void Parse_Csv_WithMultilineColumn_Works()
        {
            var path = Path.Combine("TestData", "multiline.csv");
            using var stream = File.OpenRead(path);
            var parser = new CSVExtract(stream);
            var rows = parser.Parse().ToList();
            Assert.Equal(3, rows.Count);
            Assert.Equal("Hello\r\nThis is a multiline\r\nmessage.", rows[0]["Message"]);
            Assert.Equal("Another\r\nmultiline\r\nentry", rows[1]["Message"]);
            Assert.Equal("Single line", rows[2]["Message"]);
        }
    }
}

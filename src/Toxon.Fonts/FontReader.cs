using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toxon.Fonts.Tables;

namespace Toxon.Fonts
{
    public class FontReader
    {
        public Font Read(Stream stream)
        {
            AssertStreamIsSuitable(stream);

            var reader = new FontStreamReader(stream);

            var offsetTable = ReadOffsetTable(reader);
            AssertHasAllRequiredTables(offsetTable);

            return new Font(reader, offsetTable);
        }

        private void AssertStreamIsSuitable(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException("cannot read from stream", nameof(stream));
            }
            //TODO this might be relaxed in the future
            if (!stream.CanSeek)
            {
                throw new ArgumentException("cannot seek stream", nameof(stream));
            }
        }

        private void AssertHasAllRequiredTables(OffsetTable offsetTable)
        {
            var required = new[] { "cmap", "glyf", "head", "hhea", "hmtx", "loca", "maxp", "name", "post" };
            var tables = offsetTable.Entries.Keys;

            var missingTables = required.Except(tables).ToList();

            if (missingTables.Any())
            {
                throw new FormatException("Missing tables: " + string.Join(", ", missingTables));
            }
        }

        internal OffsetTable ReadOffsetTable(FontStreamReader reader)
        {
            reader.Seek(0, SeekOrigin.Begin);

            var scalarType = reader.ReadUInt32();
            var numberOfTables = reader.ReadUInt16();

            var searchRange = reader.ReadUInt16();
            var entrySelector = reader.ReadUInt16();
            var rangeShift = reader.ReadUInt16();

            var entries = new Dictionary<string, OffsetTable.Entry>();

            for (var i = 0; i < numberOfTables; i++)
            {
                var tag = reader.ReadString(4);
                var checkSum = reader.ReadUInt32();
                var offset = reader.ReadUInt32();
                var length = reader.ReadUInt32();

                var entry = new OffsetTable.Entry(tag, checkSum, offset, length);

                //TODO assert in bounds

                if (tag != "head")
                {
                    var calculatedCheckSum = CalculateTableChecksum(reader, entry);

                    if (calculatedCheckSum != checkSum)
                    {
                        throw new FormatException(
                            $"CheckSum for '{tag}' table was {calculatedCheckSum:X} but was expected to be {checkSum:X}");
                    }
                }

                entries.Add(tag, entry);
            }

            return new OffsetTable(scalarType, entries);
        }

        private ulong CalculateTableChecksum(FontStreamReader reader, OffsetTable.Entry offsetTable)
        {
            reader.Push();
            reader.Seek(offsetTable.Offset, SeekOrigin.Begin);

            var sum = 0u;
            var count = (offsetTable.Length + 3) / 4;

            while (count-- > 0)
            {
                var value = reader.ReadUInt32();
                unchecked { sum += value; }
            }

            reader.Pop();

            return sum;
        }
    }
}

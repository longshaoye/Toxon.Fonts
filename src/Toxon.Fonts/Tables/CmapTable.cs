using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Toxon.Fonts.Tables.CmapFormats;

namespace Toxon.Fonts.Tables
{
    internal class CmapTable
    {
        public ushort Version { get; }
        //TODO proper key
        public IReadOnlyDictionary<Tuple<ushort, ushort>, CmapFormat> Subtables { get; }

        public CmapTable(ushort version, IReadOnlyDictionary<Tuple<ushort, ushort>, CmapFormat> subtables)
        {
            Version = version;
            Subtables = subtables.ToDictionary(x => x.Key, x => x.Value);
        }
        public int GetGlyphIndex(char c)
        {
            // TODO select a subtable, currently selecting the first Unicode (0)
            var subtable = Subtables.First(x => x.Key.Item1 == 0);

            return subtable.Value.GetGlyphIndex(c);
        }

        public static CmapTable Read(FontStreamReader reader, OffsetTable.Entry entry)
        {
            reader.Seek(entry);

            var version = reader.ReadUInt16();
            if (version != 0)
            {
                throw new FormatException($"Unexpected version number in cmap table: {version}, expecting 0");
            }

            var numberOfSubtables = reader.ReadUInt16();

            var subtables = new Dictionary<Tuple<ushort, ushort>, CmapFormat>();

            for (var i = 0; i < numberOfSubtables; i++)
            {
                var platformId = reader.ReadUInt16();
                var platformSpecificId = reader.ReadUInt16();

                var relOffset = reader.ReadUInt32();
                var offset = entry.Offset + relOffset;

                var subtable = ReadSubtable(reader, platformId, platformSpecificId, offset);
                subtables.Add(Tuple.Create(platformId, platformSpecificId), subtable);
            }

            return new CmapTable(version, subtables);
        }

        private static CmapFormat ReadSubtable(FontStreamReader reader, ushort platformId, ushort platformSpecificId, uint offset)
        {
            reader.Push();
            reader.Seek(offset, SeekOrigin.Begin);

            var format = reader.ReadUInt16();

            CmapFormat result;
            switch (format)
            {
                case 0: result = CmapFormat0.Read(reader, platformId, platformSpecificId); break;
                case 2: result = CmapFormat2.Read(reader, platformId, platformSpecificId); break;
                case 4: result = CmapFormat4.Read(reader, platformId, platformSpecificId); break;
                case 6: result = CmapFormat6.Read(reader, platformId, platformSpecificId); break;
                case 8: result = CmapFormat8.Read(reader, platformId, platformSpecificId); break;
                case 10: result = CmapFormat10.Read(reader, platformId, platformSpecificId); break;
                case 12: result = CmapFormat12.Read(reader, platformId, platformSpecificId); break;
                case 13: result = CmapFormat13.Read(reader, platformId, platformSpecificId); break;
                case 14: result = CmapFormat14.Read(reader, platformId, platformSpecificId); break;
                default: throw new FormatException($"Unexpected cmap subtable format: {format}");
            }

            reader.Pop();

            return result;
        }
    }
}
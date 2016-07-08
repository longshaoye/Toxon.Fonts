using System;
using Toxon.Fonts.Enums;

namespace Toxon.Fonts.Tables
{
    internal class HeadTable
    {
        public decimal Version { get; }
        public decimal FontRevision { get; }
        public ushort Flags { get; }
        public ushort UnitsPerEm { get; }
        public DateTime Created { get; }
        public DateTime Modified { get; }
        public ushort XMin { get; }
        public ushort YMin { get; }
        public ushort XMax { get; }
        public ushort YMax { get; }
        public MacStyle MACStyle { get; }
        public ushort LowestRecPPEM { get; }
        public ushort FontDirectionHint { get; }
        public IndexToLocFormat IndexToLocFormat { get; }
        public GlyphDataFormat GlyphDataFormat { get; }

        public HeadTable(decimal version, decimal fontRevision, ushort flags, ushort unitsPerEm, DateTime created, DateTime modified,
            ushort xMin, ushort yMin, ushort xMax, ushort yMax, MacStyle macStyle, ushort lowestRecPPEM, ushort fontDirectionHint,
            IndexToLocFormat indexToLocFormat, GlyphDataFormat glyphDataFormat)
        {
            Version = version;
            FontRevision = fontRevision;
            Flags = flags;
            UnitsPerEm = unitsPerEm;
            Created = created;
            Modified = modified;
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
            MACStyle = macStyle;
            LowestRecPPEM = lowestRecPPEM;
            FontDirectionHint = fontDirectionHint;
            IndexToLocFormat = indexToLocFormat;
            GlyphDataFormat = glyphDataFormat;
        }

        public static HeadTable Read(FontStreamReader reader, OffsetTable.Entry entry)
        {
            reader.Seek(entry);

            var version = reader.ReadFixed();
            var fontRevision = reader.ReadFixed();

            //TODO
            var checkSumAdjustment = reader.ReadUInt32();

            var magicNumber = reader.ReadUInt32(); // 0x5F0F3CF5
            if (magicNumber != 0x5F0F3CF5)
            {
                throw new FormatException($"Invalid magic number in header: 0x{magicNumber:X}, expecting 0x5F0F3CF5");
            }

            var flags = reader.ReadUInt16(); // bitflags

            var unitsPerEm = reader.ReadUInt16(); // 64-16384
            if (unitsPerEm < 64 || unitsPerEm > 16384)
            {
                throw new FormatException($"Invalid units per em in header: {unitsPerEm:X}, expecting a value between 64 and 16384");
            }

            var created = reader.ReadLongDateTime();
            var modified = reader.ReadLongDateTime();

            var xMin = reader.ReadFWord();
            var yMin = reader.ReadFWord();
            var xMax = reader.ReadFWord();
            var yMax = reader.ReadFWord();

            var macStyle = (MacStyle)reader.ReadUInt16(); // bitflags
            var lowestRecPPEM = reader.ReadUInt16();
            var fontDirectionHint = reader.ReadUInt16();
            var indexToLocFormat = (IndexToLocFormat)reader.ReadUInt16(); // 0 = short, 1 = long
            var glyphDataFormat = (GlyphDataFormat)reader.ReadUInt16(); // 0 = current

            return new HeadTable(version, fontRevision, flags, unitsPerEm, created, modified, xMin, yMin, xMax, yMax, macStyle, lowestRecPPEM, fontDirectionHint, indexToLocFormat, glyphDataFormat);
        }
    }
}
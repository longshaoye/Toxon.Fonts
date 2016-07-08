using System;
using Toxon.Fonts.Enums;

namespace Toxon.Fonts.Tables
{
    internal class LocaTable
    {
        private readonly uint[] offsets;

        private LocaTable(uint[] offsets)
        {
            this.offsets = offsets;
        }

        public static LocaTable Read(Font font, FontStreamReader reader, OffsetTable.Entry entry)
        {
            reader.Seek(entry);

            var length = entry.Length;
            var format = font.HeadTable.IndexToLocFormat;

            switch (format)
            {
                case IndexToLocFormat.Short:
                    return new LocaTable(reader.ReadArray<uint>(length / sizeof(ushort), r => (() => r.ReadUInt16())));
                case IndexToLocFormat.Long:
                    return new LocaTable(reader.ReadArray<uint>(length / sizeof(uint), r => r.ReadUInt32));
                default: throw new FormatException($"Invalid IndexToLocFormat: {format}");
            }
        }
    }
}
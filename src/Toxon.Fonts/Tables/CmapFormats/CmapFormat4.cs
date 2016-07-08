using System;

namespace Toxon.Fonts.Tables.CmapFormats
{
    internal class CmapFormat4 : CmapFormat
    {
        private readonly ushort language;
        private readonly ushort[] startCode;
        private readonly ushort[] endCode;
        private readonly ushort[] idDelta;
        private readonly ushort[] idRangeOffset;
        private readonly ushort[] glyphIndexArray;

        private CmapFormat4(ushort language, ushort[] startCode, ushort[] endCode, ushort[] idDelta, ushort[] idRangeOffset, ushort[] glyphIndexArray)
        {
            this.language = language;
            this.startCode = startCode;
            this.endCode = endCode;
            this.idDelta = idDelta;
            this.idRangeOffset = idRangeOffset;
            this.glyphIndexArray = glyphIndexArray;
        }

        public static CmapFormat Read(FontStreamReader reader, ushort platformId, ushort platformSpecificId)
        {
            uint length = reader.ReadUInt16();
            var language = reader.ReadUInt16();

            length -= 3 * sizeof(ushort);

            var segCount = (ushort)(reader.ReadUInt16() / 2); // stored as 2 * segCount

            var searchRange = reader.ReadUInt16();
            var entrySelector = reader.ReadUInt16();
            var rangeShift = reader.ReadUInt16();

            length -= 4 * sizeof(ushort);

            var endCode = reader.ReadArray<ushort>(segCount, r => r.ReadUInt16);
            var reservedPad = reader.ReadUInt16(); // 0
            if (reservedPad != 0)
            {
                throw new FormatException($"Unexpected value for reservedPad: {reservedPad}, expecting 0");
            }

            length -= (ushort)(segCount * sizeof(ushort) + 1);

            var startCode = reader.ReadArray<ushort>(segCount, r => r.ReadUInt16);
            var idDelta = reader.ReadArray<ushort>(segCount, r => r.ReadUInt16);
            var idRangeOffset = reader.ReadArray<ushort>(segCount, r => r.ReadUInt16);

            length -= (ushort)(3 * segCount * sizeof(ushort));

            var glyphIndexArray = reader.ReadArray<ushort>(length / sizeof(ushort), r => r.ReadUInt16);

            return new CmapFormat4(language, startCode, endCode, idDelta, idRangeOffset, glyphIndexArray);
        }
    }
}
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

        public override int GetGlyphIndex(char c)
        {
            var ci = (ushort)c;

            // find first endCode > c
            var i = 0;
            for (; i < endCode.Length; i++)
            {
                if (endCode[i] > ci)
                {
                    break;
                }
            }

            var segStart = startCode[i];
            if (segStart > ci)
            {
                return Missing;
            }

            var delta = idDelta[i];
            var offset = idRangeOffset[i];

            if (offset == 0)
            {
                return (delta + ci) % 0xffff;
            }

            // offset is in bytes
            var glyphIndexIndex = offset / 2 + (c - segStart);
            // this is relative to the idRangeOffsetEntry, so adjust it to array index
            glyphIndexIndex -= glyphIndexArray.Length - i;

            var glyphIndex = glyphIndexArray[glyphIndexIndex];

            if (glyphIndex == Missing)
            {
                return Missing;
            }

            return (delta + ci) % 0xffff;
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
            if (endCode[endCode.Length - 1] != 0xffff)
            {
                throw new FormatException($"Unexpected last endCode: 0x{endCode[endCode.Length - 1]:X}, expecting 0xffff");
            }

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
using System;

namespace Toxon.Fonts.Tables.CmapFormats
{
    internal class CmapFormat2 : CmapFormat
    {
        public static CmapFormat Read(FontStreamReader reader, ushort platformId, ushort platformSpecificId)
        {
            throw new NotImplementedException();
        }

        public override int GetGlyphIndex(char c)
        {
            throw new NotImplementedException();
        }
    }
}
namespace Toxon.Fonts.Tables.CmapFormats
{
    internal class CmapFormat0 : CmapFormat
    {
        private readonly ushort language;
        private readonly byte[] glyphIndexArray;

        private CmapFormat0(ushort language, byte[] glyphIndexArray)
        {
            this.language = language;
            this.glyphIndexArray = glyphIndexArray;
        }

        public static CmapFormat Read(FontStreamReader reader, ushort platformId, ushort platformSpecificId)
        {
            var length = reader.ReadUInt16();
            var language = reader.ReadUInt16();

            var glyphIndexArray = reader.ReadArray<byte>(256, r => r.ReadUInt8);

            return new CmapFormat0(language, glyphIndexArray);
        }

        public override int GetGlyphIndex(char c)
        {
            throw new System.NotImplementedException();
        }
    }
}

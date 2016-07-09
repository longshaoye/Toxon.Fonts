using System;
using Toxon.Fonts.Glyphs;
using Toxon.Fonts.Tables;

namespace Toxon.Fonts
{
    public class Font
    {
        // reading
        private OffsetTable offsetTable;
        private FontStreamReader reader;

        // tables
        internal HeadTable HeadTable { get; }
        private readonly FpgmTable fpgmTable;
        private readonly CmapTable cmapTable;
        private readonly LocaTable locaTable;
        private readonly GlyfTable glyfTable;

        internal Font(FontStreamReader reader, OffsetTable offsetTable)
        {
            this.reader = reader;
            this.offsetTable = offsetTable;

            HeadTable = HeadTable.Read(reader, offsetTable.Entries["head"]);
            if (offsetTable.Entries.ContainsKey("fpgm"))
            {
                fpgmTable = FpgmTable.Read(reader, offsetTable.Entries["fpgm"]);
            }
            cmapTable = CmapTable.Read(reader, offsetTable.Entries["cmap"]);
            locaTable = LocaTable.Read(this, reader, offsetTable.Entries["loca"]);
            glyfTable = GlyfTable.Read(reader, offsetTable.Entries["glyf"]);

            //TODO run fpgm
        }

        public Glyph GetGlpyh(char c)
        {
            // cmap
            var glyphIndex = cmapTable.GetGlyphIndex(c);

            // loca would be needed if glyf is dynamically read

            // glyf
            return glyfTable.GetGlyph(glyphIndex);
        }
    }
}

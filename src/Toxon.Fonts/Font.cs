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
        private CmapTable cmapTable;
        private LocaTable locaTable;
        private GlyfTable glyfTable;

        internal Font(FontStreamReader reader, OffsetTable offsetTable)
        {
            this.reader = reader;
            this.offsetTable = offsetTable;

            HeadTable = HeadTable.Read(reader, offsetTable.Entries["head"]);
            cmapTable = CmapTable.Read(reader, offsetTable.Entries["cmap"]);
            locaTable = LocaTable.Read(this, reader, offsetTable.Entries["loca"]);
            glyfTable = GlyfTable.Read(reader, offsetTable.Entries["glyf"]);

        }
    }
}

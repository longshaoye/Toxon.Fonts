using Toxon.Fonts.Tables;

namespace Toxon.Fonts.Glyphs
{
    internal class SimpleGlyph : Glyph
    {
        private readonly ushort xMin;
        private readonly ushort yMin;
        private readonly ushort xMax;
        private readonly ushort yMax;

        private readonly ushort[] endPoints;
        private readonly byte[] instructions;
        private readonly GlyfTable.Point[] points;

        public SimpleGlyph(ushort xMin, ushort yMin, ushort xMax, ushort yMax,
            ushort[] endPoints, byte[] instructions, GlyfTable.Point[] points)
        {
            this.xMin = xMin;
            this.yMin = yMin;
            this.xMax = xMax;
            this.yMax = yMax;

            this.endPoints = endPoints;
            this.instructions = instructions;
            this.points = points;
        }
    }
}

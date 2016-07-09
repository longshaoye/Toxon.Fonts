using Toxon.Fonts.Rendering;

namespace Toxon.Fonts.Glyphs
{
    public abstract class Glyph
    {
        public abstract void Render(FontRenderer renderer, Point origin);
    }
}

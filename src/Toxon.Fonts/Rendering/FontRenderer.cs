using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toxon.Fonts.Rendering
{
    public class FontRenderer
    {
        private readonly Font font;

        public FontRenderer(Font font)
        {
            this.font = font;
        }

        public FontImage Render(char c)
        {
            var glyph = font.GetGlpyh(c);

            throw new NotImplementedException();
        }
    }
}

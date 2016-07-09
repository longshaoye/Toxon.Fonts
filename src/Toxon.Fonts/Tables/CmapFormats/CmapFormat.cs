using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toxon.Fonts.Tables.CmapFormats
{
    internal abstract class CmapFormat
    {
        public static readonly int Missing = 0;

        public abstract int GetGlyphIndex(char c);
    }
}

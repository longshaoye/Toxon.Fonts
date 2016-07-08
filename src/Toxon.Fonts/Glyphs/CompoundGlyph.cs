﻿using System.Collections.Generic;
using System.Linq;

namespace Toxon.Fonts.Glyphs
{
    internal class CompoundGlyph : Glyph
    {
        private readonly IReadOnlyList<Component> components;
        private readonly byte[] instructions;

        public CompoundGlyph(IEnumerable<Component> components, byte[] instructions)
        {
            this.components = components.ToList();
            this.instructions = instructions;
        }

        public class Component
        {
            private short glyphIndex;
            private decimal[] matrix;

            private short? destPointIndex;
            private short? srcPointIndex;

            public Component(short glyphIndex, decimal[] matrix, short? destPointIndex, short? srcPointIndex)
            {
                this.glyphIndex = glyphIndex;
                this.matrix = matrix;
                this.destPointIndex = destPointIndex;
                this.srcPointIndex = srcPointIndex;
            }
        }
    }
}

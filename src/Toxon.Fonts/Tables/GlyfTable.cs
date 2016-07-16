using System;
using System.Collections.Generic;
using System.Linq;
using Toxon.Fonts.Enums;
using Toxon.Fonts.Glyphs;

namespace Toxon.Fonts.Tables
{
    internal class GlyfTable
    {
        public IReadOnlyList<Glyph> Glyphs { get; }

        private GlyfTable(IEnumerable<Glyph> glyphs)
        {
            Glyphs = glyphs.ToList();
        }

        public Glyph GetGlyph(int glyphIndex)
        {
            return Glyphs[glyphIndex];
        }

        public static GlyfTable Read(FontStreamReader reader, OffsetTable.Entry entry)
        {
            reader.Seek(entry);

            var glyphs = new List<Glyph>();
            while (reader.Position < entry.Offset + entry.Length)
            {
                glyphs.Add(ReadGlyph(reader));

                while ((reader.Position & 1) != 0)
                {
                    reader.ReadUInt8();
                }
            }

            if (reader.Position > entry.Offset + entry.Length)
            {
                throw new Exception("Overflow?");
            }

            return new GlyfTable(glyphs);
        }

        private static Glyph ReadGlyph(FontStreamReader reader)
        {
            var numberOfContours = reader.ReadInt16();
            var isCompound = numberOfContours < 0;

            var xMin = reader.ReadFWord();
            var yMin = reader.ReadFWord();
            var xMax = reader.ReadFWord();
            var yMax = reader.ReadFWord();

            if (isCompound)
            {
                return ReadCompoundGlyph(reader, xMin, yMin, xMax, yMax);
            }

            return ReadSimpleGlyph(reader, numberOfContours, xMin, yMin, xMax, yMax);
        }

        private static Glyph ReadCompoundGlyph(FontStreamReader reader, ushort xMin, ushort yMin, ushort xMax, ushort yMax)
        {
            var components = new List<CompoundGlyph.Component>();

            var flags = GlyphComponentFlags.MoreComponents;
            while (flags.HasFlag(GlyphComponentFlags.MoreComponents))
            {
                flags = (GlyphComponentFlags)reader.ReadUInt16();
                var glyphIndex = reader.ReadInt16();

                if (flags.HasFlag(GlyphComponentFlags.WeHaveInstructions) && flags.HasFlag(GlyphComponentFlags.MoreComponents))
                {
                    throw new NotImplementedException();
                }

                var arg1 = flags.HasFlag(GlyphComponentFlags.Arg12AreWords) ? reader.ReadInt16() : reader.ReadUInt8();
                var arg2 = flags.HasFlag(GlyphComponentFlags.Arg12AreWords) ? reader.ReadInt16() : reader.ReadUInt8();

                var matrix = new decimal[]{
                    1, 0,
                    0, 1,
                    0, 0
                };

                short? destPointIndex = null;
                short? srcPointIndex = null;

                if (flags.HasFlag(GlyphComponentFlags.ArgsAreXYValues))
                {
                    matrix[4] = arg1;
                    matrix[5] = arg2;
                }
                else
                {
                    destPointIndex = arg1;
                    srcPointIndex = arg1;
                }

                if (flags.HasFlag(GlyphComponentFlags.WeHaveAScale))
                {
                    matrix[0] = matrix[3] = reader.ReadF2Dot14();
                }
                else if (flags.HasFlag(GlyphComponentFlags.WeHaveAnXYScale))
                {
                    matrix[0] = reader.ReadF2Dot14();
                    matrix[3] = reader.ReadF2Dot14();
                }
                else if (flags.HasFlag(GlyphComponentFlags.WeHave2By2))
                {
                    matrix[0] = reader.ReadF2Dot14();
                    matrix[1] = reader.ReadF2Dot14();
                    matrix[2] = reader.ReadF2Dot14();
                    matrix[3] = reader.ReadF2Dot14();
                }

                components.Add(new CompoundGlyph.Component(glyphIndex, matrix, destPointIndex, srcPointIndex));
            }

            var instructions = new byte[0];
            if (flags.HasFlag(GlyphComponentFlags.WeHaveInstructions))
            {
                var instructionLength = reader.ReadUInt16();
                instructions = reader.ReadArray<byte>(instructionLength, r => r.ReadUInt8);
            }

            return new CompoundGlyph(components, instructions);
        }

        private static Glyph ReadSimpleGlyph(FontStreamReader reader, short numberOfContours, ushort xMin, ushort yMin, ushort xMax, ushort yMax)
        {
            var endPoints = reader.ReadArray<ushort>((ushort)numberOfContours, r => r.ReadUInt16);

            var instructionLength = reader.ReadUInt16();
            var instructions = reader.ReadArray<byte>(instructionLength, r => r.ReadUInt8);

            if (numberOfContours == 0)
            {
                return new SimpleGlyph(xMin, yMin, xMax, yMax, endPoints, instructions, new Point[0]);
            }

            var numberOfPoints = endPoints.Max() + 1;

            var flags = new SimpleGlyphFlags[numberOfPoints];
            var points = new Point[numberOfPoints];

            for (var i = 0; i < numberOfPoints; i++)
            {
                var flag = (SimpleGlyphFlags)reader.ReadUInt8();
                flags[i] = flag;
                points[i] = new Point { OnCurve = flag.HasFlag(SimpleGlyphFlags.OnCurve) };

                if (flag.HasFlag(SimpleGlyphFlags.Repeat))
                {
                    var count = reader.ReadUInt8();
                    while (count-- > 0)
                    {
                        flags[++i] = flag;
                        points[i] = new Point { OnCurve = flag.HasFlag(SimpleGlyphFlags.OnCurve) };
                    }
                }
            }

            // read X
            short value = 0;
            for (var i = 0; i < numberOfPoints; i++)
            {
                var flag = flags[i];

                if (flag.HasFlag(SimpleGlyphFlags.XShort))
                {
                    if (flag.HasFlag(SimpleGlyphFlags.XSame))
                    {
                        value += reader.ReadUInt8();
                    }
                    else
                    {
                        value -= reader.ReadUInt8();
                    }
                }
                else if (!flag.HasFlag(SimpleGlyphFlags.XSame))
                {
                    value += reader.ReadInt16();
                }

                points[i].X = value;
            }

            // read Y
            value = 0;
            for (var i = 0; i < numberOfPoints; i++)
            {
                var flag = flags[i];

                if (flag.HasFlag(SimpleGlyphFlags.YShort))
                {
                    if (flag.HasFlag(SimpleGlyphFlags.YSame))
                    {
                        value += reader.ReadUInt8();
                    }
                    else
                    {
                        value -= reader.ReadUInt8();
                    }
                }
                else if (!flag.HasFlag(SimpleGlyphFlags.YSame))
                {
                    value += reader.ReadInt16();
                }

                points[i].Y = value;
            }

            return new SimpleGlyph(xMin, yMin, xMax, yMax, endPoints, instructions, points);
        }

        internal class Point
        {
            public int X { get; internal set; }
            public int Y { get; internal set; }

            public bool OnCurve { get; internal set; }

            public override string ToString()
            {
                return $"X:{X}, Y:{Y}, OnCurve:{OnCurve}";
            }
        }
    }
}
using System;
using Toxon.Fonts.Rendering;
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

        public override void Render(FontRenderer renderer, Point origin)
        {
            var currentEndPoint = 0;

            var firstInSequence = true;
            var sequenceOrigin = new Point(0, 0);

            BezierCurve bezier = null;

            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var renderPoint = new Point(point.X, point.Y);

                if (firstInSequence)
                {
                    bezier = new BezierCurve();
                    renderer.MoveTo(renderPoint);

                    firstInSequence = false;
                    sequenceOrigin = renderPoint;
                }

                bezier.AddPoint(renderPoint - sequenceOrigin);

                if (point.OnCurve && bezier.Points.Count > 1)
                {
                    renderer.Draw(bezier);
                    bezier = new BezierCurve();
                    bezier.AddPoint(renderPoint - sequenceOrigin);
                }

                if (endPoints[currentEndPoint] == i)
                {
                    if (!point.OnCurve)
                    {
                        // last point is off-curve, loop to origin
                        bezier.AddPoint(new Point(0, 0));
                        renderer.Draw(bezier);
                    }

                    firstInSequence = true;
                    currentEndPoint++;
                }
            }
        }
    }
}

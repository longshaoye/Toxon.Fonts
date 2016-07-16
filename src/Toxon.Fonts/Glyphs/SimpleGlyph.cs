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
            var currentState = RenderState.Initial;

            var sequenceStartPoint = default(Point);

            Point initial = default(Point);
            Point control = default(Point);

            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var renderPoint = new Point(point.X, point.Y);

                switch (currentState)
                {
                    case RenderState.Initial:
                        if (!point.OnCurve)
                        {
                            throw new FormatException("Initial point should be on curve");
                        }

                        initial = renderPoint;
                        sequenceStartPoint = initial;
                        currentState = RenderState.HaveStartPoint;
                        break;
                    case RenderState.HaveStartPoint:
                        if (point.OnCurve)
                        {
                            renderer.DrawLine(initial, renderPoint);

                            initial = renderPoint;
                            currentState = RenderState.HaveStartPoint;
                        }
                        else
                        {
                            control = renderPoint;
                            currentState = RenderState.HaveControlPoint;
                        }
                        break;
                    case RenderState.HaveControlPoint:
                        if (point.OnCurve)
                        {
                            var curve = new BezierCurve();

                            curve.AddPoint(initial);
                            curve.AddPoint(control);
                            curve.AddPoint(renderPoint);

                            renderer.DrawCurve(curve);

                            initial = renderPoint;
                            currentState = RenderState.HaveStartPoint;
                        }
                        else
                        {
                            var midPoint = (control + renderPoint) / 2;

                            var curve = new BezierCurve();

                            curve.AddPoint(initial);
                            curve.AddPoint(control);
                            curve.AddPoint(midPoint);

                            renderer.DrawCurve(curve);

                            initial = midPoint;
                            control = renderPoint;
                            currentState = RenderState.HaveControlPoint;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (endPoints[currentEndPoint] == i)
                {
                    if (currentState == RenderState.HaveControlPoint)
                    {
                        var curve = new BezierCurve();

                        curve.AddPoint(initial);
                        curve.AddPoint(control);
                        curve.AddPoint(sequenceStartPoint);

                        renderer.DrawCurve(curve);
                    }

                    if (currentState == RenderState.Initial)
                    {
                        throw new FormatException("Ending an empty curve");
                    }

                    sequenceStartPoint = default(Point);
                    initial = default(Point);
                    control = default(Point);
                    currentState = RenderState.Initial;

                    currentEndPoint++;
                }
            }

            if (currentState != RenderState.Initial)
            {
                throw new FormatException("Trailing points found");
            }
        }

        private enum RenderState
        {
            Initial,
            HaveStartPoint,
            HaveControlPoint,
        }
    }
}

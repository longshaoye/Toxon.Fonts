using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Toxon.Fonts.Rendering.Instructions
{
    internal class DrawBezierCurveInstruction : RenderInstruction
    {
        private readonly BezierCurve bezier;

        public DrawBezierCurveInstruction(BezierCurve bezier)
        {
            this.bezier = bezier;
        }

        public override void Render(FontImage img)
        {
            var points = bezier.Points;
            IEnumerable<RenderInstruction> instructions = Enumerable.Empty<RenderInstruction>();

            for (var i = 0; i < points.Count - 2; i += 2)
            {
                var p0 = points[i];
                var p1 = points[i + 1];
                var p2 = points[i + 2];

                var x0 = (int)p0.X;
                var y0 = (int)p0.Y;
                var x1 = (int)p1.X;
                var y1 = (int)p1.Y;
                var x2 = (int)p2.X;
                var y2 = (int)p2.Y;

                instructions = instructions.Concat(PlotQuadBezier(x0, y0, x1, y1, x2, y2));
            }

            var instructionList = instructions.ToList();

            foreach (var instruction in instructionList)
            {
                instruction.Render(img);
            }
        }

        private IEnumerable<RenderInstruction> PlotQuadBezier(int x0, int y0, int x1, int y1, int x2, int y2)
        {
            var instructions = Enumerable.Empty<RenderInstruction>();

            int x = x0 - x1, y = y0 - y1;
            double t = x0 - 2 * x1 + x2, r;

            if ((long)x * (x2 - x1) > 0)
            {
                if ((long)y * (y2 - y1) > 0)
                {
                    if (Math.Abs((y0 - 2 * y1 + y2) / t * x) > Math.Abs(y))
                    {
                        x0 = x2;
                        x2 = x + x1;
                        y0 = y2;
                        y2 = y + y1;
                    }
                }

                t = (x0 - x1) / t;
                r = (1 - t) * ((1 - t) * y0 + 2.0 * t * y1) + t * t * y2;
                t = (x0 * x2 - x1 * x1) * t / (x0 - x1);
                x = Floor(t + 0.5); y = Floor(r + 0.5);
                r = (y1 - y0) * (t - x0) / (x1 - x0) + y0;

                instructions = instructions.Concat(PlotQuadBezierSeg(x0, y0, x, Floor(r + 0.5), x, y));

                r = (y1 - y2) * (t - x2) / (x1 - x2) + y2;
                x0 = x1 = x; y0 = y; y1 = Floor(r + 0.5);
            }
            if ((long)(y0 - y1) * (y2 - y1) > 0)
            {
                t = y0 - 2 * y1 + y2; t = (y0 - y1) / t;
                r = (1 - t) * ((1 - t) * x0 + 2.0 * t * x1) + t * t * x2;
                t = (y0 * y2 - y1 * y1) * t / (y0 - y1);
                x = Floor(r + 0.5); y = Floor(t + 0.5);
                r = (x1 - x0) * (t - y0) / (y1 - y0) + x0;
                instructions = instructions.Concat(PlotQuadBezierSeg(x0, y0, Floor(r + 0.5), y, x, y));
                r = (x1 - x2) * (t - y2) / (y1 - y2) + x2;
                x0 = x; x1 = Floor(r + 0.5); y0 = y1 = y;
            }

            instructions = instructions.Concat(PlotQuadBezierSeg(x0, y0, x1, y1, x2, y2));

            return instructions;
        }

        private int Floor(double d)
        {
            return (int)Math.Floor(d);
        }

        private IEnumerable<RenderInstruction> PlotQuadBezierSeg(int x0, int y0, int x1, int y1, int x2, int y2)
        {
            int sx = x2 - x1, sy = y2 - y1;
            long xx = x0 - x1, yy = y0 - y1;

            double cur = xx * sy - yy * sx;

            Debug.Assert(xx * sx <= 0 && yy * sy <= 0);

            if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
            {
                x2 = x0; x0 = sx + x1; y2 = y0; y0 = sy + y1; cur = -cur;
            }

            if (cur != 0)
            {
                xx += sx; xx *= sx = x0 < x2 ? 1 : -1;
                yy += sy; yy *= sy = y0 < y2 ? 1 : -1;

                var xy = 2 * xx * yy;
                xx *= xx;
                yy *= yy;

                if (cur * sx * sy < 0)
                {
                    xx = -xx; yy = -yy; xy = -xy; cur = -cur;
                }

                var dx = 4.0 * sy * cur * (x1 - x0) + xx - xy;
                var dy = 4.0 * sx * cur * (y0 - y1) + yy - xy;

                xx += xx;
                yy += yy;

                var err = dx + dy + xy;

                do
                {
                    yield return new DrawPixelInstruction(new Point(x0, y0));
                    if (x0 == x2 && y0 == y2)
                    {
                        yield break;
                    }

                    y1 = 2 * err < dx ? 1 : 0;

                    if (2 * err > dy)
                    {
                        x0 += sx;
                        dx -= xy;
                        err += dy += yy;
                    }
                    if (y1 != 0)
                    {
                        y0 += sy;
                        dy -= xy;
                        err += dx += xx;
                    }
                } while (dy < 0 && dx > 0);
            }

            yield return new DrawLineInstruction(new Point(x0, y0), new Point(x2, y2));
        }
    }
}

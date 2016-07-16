using System;

namespace Toxon.Fonts.Rendering.Instructions
{
    internal class DrawLineInstruction : RenderInstruction
    {
        private readonly Point start;
        private readonly Point end;

        public DrawLineInstruction(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        public override void Render(FontImage img)
        {
            var x0 = start.X;
            var y0 = start.Y;
            var x1 = end.X;
            var y1 = end.Y;

            var dx = Math.Abs(x1 - x0);
            var dy = -Math.Abs(y1 - y0);

            var sx = x0 < x1 ? 1 : -1;
            var sy = y0 < y1 ? 1 : -1;

            var err = dx + dy;

            for (;;)
            {
                img.SetPixel(new Point(x0, y0));
                if (Math.Abs(x1 - x0) < 1 && Math.Abs(y1 - y0) < 1)
                {
                    break;
                }

                var e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
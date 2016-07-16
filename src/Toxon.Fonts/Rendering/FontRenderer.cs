using Toxon.Fonts.Rendering.Instructions;

namespace Toxon.Fonts.Rendering
{
    public class FontRenderer
    {
        private readonly Font font;
        private FontImage image;

        public FontRenderer(Font font)
        {
            this.font = font;
        }

        public FontImage Render(char c)
        {
            image = new FontImage();

            var glyph = font.GetGlpyh(c);
            glyph.Render(this, new Point(0, 0));

            return image;
        }

        public void MoveTo(Point point)
        {
            image.AddInstruction(new MoveToInstruction(point));
        }

        public void DrawCurve(BezierCurve bezier)
        {
            image.AddInstruction(new DrawBezierCurveInstruction(bezier));
        }

        public void DrawLine(Point start, Point end)
        {
            image.AddInstruction(new DrawLineInstruction(start, end));
        }
    }
}

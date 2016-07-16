namespace Toxon.Fonts.Rendering.Instructions
{
    internal class DrawPixelInstruction : RenderInstruction
    {
        private readonly Point point;

        public DrawPixelInstruction(Point point)
        {
            this.point = point;
        }

        public override void Render(FontImage img)
        {
            img.SetPixel(point);
        }
    }
}
namespace Toxon.Fonts.Rendering.Instructions
{
    internal class MoveToInstruction : RenderInstruction
    {
        private readonly Point point;

        public MoveToInstruction(Point point)
        {
            this.point = point;
        }

        public override void Render(FontImage img)
        {
            throw new System.NotImplementedException();
        }
    }
}

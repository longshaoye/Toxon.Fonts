namespace Toxon.Fonts.Rendering.Instructions
{
    internal class MoveToInstruction : RenderInstruction
    {
        private readonly Point point;

        public MoveToInstruction(Point point)
        {
            this.point = point;
        }
    }
}

namespace Toxon.Fonts.Rendering.Instructions
{
    internal class DrawBezierCurveInstruction : RenderInstruction
    {
        private readonly BezierCurve bezier;

        public DrawBezierCurveInstruction(BezierCurve bezier)
        {
            this.bezier = bezier;
        }
    }
}

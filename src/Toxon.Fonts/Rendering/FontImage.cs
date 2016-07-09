using System.Collections.Generic;
using Toxon.Fonts.Rendering.Instructions;

namespace Toxon.Fonts.Rendering
{
    public class FontImage
    {
        private readonly List<RenderInstruction> instructions = new List<RenderInstruction>();

        internal void AddInstruction(RenderInstruction instruction)
        {
            instructions.Add(instruction);
        }
    }
}
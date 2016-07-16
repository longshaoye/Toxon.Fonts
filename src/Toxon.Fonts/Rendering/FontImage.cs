using System;
using System.Collections.Generic;
using System.IO;
using Toxon.Fonts.Rendering.Instructions;

namespace Toxon.Fonts.Rendering
{
    public class FontImage
    {
        private readonly List<RenderInstruction> instructions = new List<RenderInstruction>();

        private readonly Dictionary<Point, Color> pixels = new Dictionary<Point, Color>();

        public byte[] Render()
        {
            ProcessInstructions();

        }

        internal void SetPixel(Point point)
        {
            //TODO take from current render state
            SetPixel(point, Color.Black);
        }
        internal void SetPixel(Point point, Color color)
        {
            var normalizedPoint = Normalize(point);
            pixels[normalizedPoint] = color;
        }

        private Point Normalize(Point point)
        {
            return new Point((int)point.X, (int)point.Y);
        }

        internal void AddInstruction(RenderInstruction instruction)
        {
            instructions.Add(instruction);
        }

        private void ProcessInstructions()
        {
            foreach (var instruction in instructions)
            {
                instruction.Render(this);
            }

            instructions.Clear();
        }

        private Rectangle GetBounds()
        {
            var minX = 0m;
            var minY = 0m;
            var maxX = 0m;
            var maxY = 0m;

            foreach (var point in pixels.Keys)
            {
                if (point.X < minX)
                {
                    minX = point.X;
                }
                else if (point.X > maxX)
                {
                    maxX = point.X;
                }

                if (point.Y < minY)
                {
                    minY = point.Y;
                }
                else if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
            }

            return new Rectangle(new Point(minX, minY), new Size(maxX - minX, maxY - minY));
        }
    }
}
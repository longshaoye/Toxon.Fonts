using System.Collections.Generic;

namespace Toxon.Fonts.Rendering
{
    public class BezierCurve
    {
        private readonly List<Point> points = new List<Point>();
        public IReadOnlyList<Point> Points => points;

        public BezierCurve() { }

        public void AddPoint(Point point)
        {
            points.Add(point);
        }
    }
}

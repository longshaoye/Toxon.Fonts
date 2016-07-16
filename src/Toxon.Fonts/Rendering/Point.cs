namespace Toxon.Fonts.Rendering
{
    public struct Point
    {
        public decimal X { get; }
        public decimal Y { get; }

        public Point(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
        public static Point operator *(int scalar, Point p)
        {
            return new Point(scalar * p.X, scalar * p.Y);
        }
        public static Point operator *(Point p, int scalar)
        {
            return new Point(scalar * p.X, scalar * p.Y);
        }
        public static Point operator /(Point p, int scalar)
        {
            return new Point(p.X / scalar, p.Y / scalar);
        }

        public static Point operator -(Point a)
        {
            return new Point(-a.X, -a.Y);
        }


        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }

            var other = (Point)obj;

            return Equals(other);
        }
        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}";
        }
    }
}

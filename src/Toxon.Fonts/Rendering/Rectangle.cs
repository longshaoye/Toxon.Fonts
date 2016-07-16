namespace Toxon.Fonts.Rendering
{
    internal struct Rectangle
    {
        public Point TopLeft { get; }
        public Size Size { get; }

        public Rectangle(Point topLeft, Size size)
        {
            TopLeft = topLeft;
            Size = size;
        }

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
            {
                return false;
            }

            var other = (Rectangle)obj;

            return Equals(other);
        }
        public bool Equals(Rectangle other)
        {
            return TopLeft == other.TopLeft && Size == other.Size;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TopLeft.GetHashCode() * 397) ^ Size.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"TopLeft:{TopLeft}, Size:{Size}";
        }

    }
}
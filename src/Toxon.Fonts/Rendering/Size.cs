namespace Toxon.Fonts.Rendering
{
    public struct Size
    {
        public Size(decimal width, decimal height)
        {
            Width = width;
            Height = height;
        }

        public decimal Width { get; }
        public decimal Height { get; }

        public static bool operator ==(Size a, Size b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Size a, Size b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size))
            {
                return false;
            }

            return Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"Width:{Width}, Height:{Height}";
        }
    }
}
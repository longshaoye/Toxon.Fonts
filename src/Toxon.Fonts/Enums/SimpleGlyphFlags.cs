using System;

namespace Toxon.Fonts.Enums
{
    [Flags]
    public enum SimpleGlyphFlags
    {
        None = 0,

        OnCurve = 0x1,
        XShort = 0x2,
        YShort = 0x4,
        Repeat = 0x8,
        XSame = 0x10,
        YSame = 0x20,
    }
}

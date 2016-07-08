using System;

namespace Toxon.Fonts.Enums
{
    [Flags]
    internal enum MacStyle
    {
        None = 0,

        Bold = 0x1,
        Italic = 0x2,
        Underline = 0x4,
        Outline = 0x8,
        Shadow = 0x10,
        Condensed = 0x20,
        Extended = 0x40,
    }
}
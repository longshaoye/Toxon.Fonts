using System;

namespace Toxon.Fonts.Enums
{
    [Flags]
    public enum GlyphComponentFlags
    {
        None = 0,

        Arg12AreWords = 0x1,
        ArgsAreXYValues = 0x2,
        RoundXYToGrid = 0x4,
        WeHaveAScale = 0x8,
        MoreComponents = 0x20,
        WeHaveAnXYScale = 0x40,
        WeHave2By2 = 0x80,
        WeHaveInstructions = 0x100,
        UseMyMetrics = 0x200,
        OverlapCompound = 0x400,
    }
}

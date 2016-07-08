using System.Collections.Generic;
using System.Linq;

namespace Toxon.Fonts.Tables
{
    internal class OffsetTable
    {
        public OffsetTable(uint scalarType, IReadOnlyDictionary<string, Entry> entries)
        {
            ScalarType = scalarType;
            Entries = entries.ToDictionary(x => x.Key, x => x.Value);
        }
        
        public uint ScalarType { get; }
        public IReadOnlyDictionary<string, Entry> Entries { get; }

        internal class Entry
        {
            public string Tag { get; }
            public uint CheckSum { get; }
            public uint Offset { get; }
            public uint Length { get; }

            public Entry(string tag, uint checkSum, uint offset, uint length)
            {
                Tag = tag;
                CheckSum = checkSum;
                Offset = offset;
                Length = length;
            }
        }
    }
}

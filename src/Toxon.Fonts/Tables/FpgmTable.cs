namespace Toxon.Fonts.Tables
{
    internal class FpgmTable
    {
        private readonly byte[] instructions;

        private FpgmTable(byte[] instructions)
        {
            this.instructions = instructions;
        }

        public static FpgmTable Read(FontStreamReader reader, OffsetTable.Entry entry)
        {
            reader.Seek(entry);

            var instructions = reader.ReadArray<byte>(entry.Length, r => r.ReadUInt8);

            return new FpgmTable(instructions);
        }
    }
}

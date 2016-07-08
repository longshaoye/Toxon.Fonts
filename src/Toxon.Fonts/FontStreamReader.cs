using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toxon.Fonts.Tables;

namespace Toxon.Fonts
{
    internal class FontStreamReader
    {
        private readonly Stream stream;

        public FontStreamReader(Stream stream)
        {
            this.stream = stream;
        }

        #region seeking

        public long Position => stream.Position;

        public void Seek(long offset, SeekOrigin origin)
        {
            stream.Seek(offset, origin);
        }

        internal void Seek(OffsetTable.Entry entry)
        {
            Seek(entry.Offset, SeekOrigin.Begin);
        }

        private readonly Stack<long> offsetStack = new Stack<long>();

        public void Push()
        {
            offsetStack.Push(stream.Position);
        }

        public void Pop()
        {
            Seek(offsetStack.Pop(), SeekOrigin.Begin);
        }

        #endregion

        public long ReadInt64()
        {
            var buffer = new byte[sizeof(long)];

            stream.Read(buffer, 0, sizeof(long));

            return (buffer[0] << 56) | (buffer[1] << 48) | (buffer[2] << 40) | (buffer[3] << 32) | (buffer[4] << 24) | (buffer[5] << 16) | (buffer[6] << 8) | buffer[7];
        }
        public int ReadInt32()
        {
            var buffer = new byte[sizeof(int)];

            stream.Read(buffer, 0, sizeof(int));

            return (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];
        }

        public short ReadInt16()
        {
            var buffer = new byte[sizeof(short)];

            stream.Read(buffer, 0, sizeof(short));

            return (short)((buffer[0] << 8) | buffer[1]);
        }

        public sbyte ReadInt8()
        {
            var buffer = new byte[sizeof(sbyte)];

            stream.Read(buffer, 0, sizeof(sbyte));

            return (sbyte)buffer[0];
        }


        public ulong ReadUInt64()
        {
            var buffer = new byte[sizeof(ulong)];

            stream.Read(buffer, 0, sizeof(ulong));

            return (ulong)((buffer[0] << 56) | (buffer[1] << 48) | (buffer[2] << 40) | (buffer[3] << 32) | (buffer[4] << 24) | (buffer[5] << 16) | (buffer[6] << 8) | buffer[7]);
        }
        public uint ReadUInt32()
        {
            var buffer = new byte[sizeof(uint)];

            stream.Read(buffer, 0, sizeof(uint));

            return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
        }

        public ushort ReadUInt16()
        {
            var buffer = new byte[sizeof(ushort)];

            stream.Read(buffer, 0, sizeof(ushort));

            return (ushort)((buffer[0] << 8) | buffer[1]);
        }

        public byte ReadUInt8()
        {
            var buffer = new byte[sizeof(byte)];

            stream.Read(buffer, 0, sizeof(byte));

            return buffer[0];
        }

        public ushort ReadFWord()
        {
            return ReadUInt16();
        }

        public string ReadString(int length)
        {
            var buffer = new byte[length];

            stream.Read(buffer, 0, length);

            return Encoding.ASCII.GetString(buffer);
        }

        public decimal ReadFixed()
        {
            const decimal offset = 1 << 16;

            var value = ReadInt32();

            return value / offset;
        }

        public decimal ReadF2Dot14()
        {
            const decimal offset = 1 << 14;

            var value = ReadInt16();

            return value / offset;
        }

        public DateTime ReadLongDateTime()
        {
            var offset = new DateTime(1904, 1, 1, 0, 0, 0);
            var value = ReadInt64();

            return offset.AddSeconds(value);
        }

        public delegate T ArraySelector<out T>();
        public delegate ArraySelector<T> ArraySelectorFactory<out T>(FontStreamReader reader);

        public T[] ReadArray<T>(uint count, ArraySelectorFactory<T> func)
        {
            var array = new T[count];
            var selector = func(this);

            for (var i = 0; i < count; i++)
            {
                array[i] = selector();
            }

            return array;
        }
    }
}
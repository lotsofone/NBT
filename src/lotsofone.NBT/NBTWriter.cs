using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace lotsofone.NBT
{
    public class NBTWriter : IDisposable
    {
        Stream _stream;
        byte[] _buffer = new byte[8];
        public NBTWriter(Stream stream)
        {
            this._stream = stream;
        }
        #region WriteSymbols
        public class HalfTagCloser : IDisposable
        {
            NBTWriter writer;
            TagType type;
            public HalfTagCloser(NBTWriter writer, TagType type) { this.writer = writer; this.type = type; }

            #region IDisposable Support
            private bool disposedValue = false; // 要检测冗余调用

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        if(type==TagType.Compound) writer.WriteTagEnd();
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                    this.writer = null;

                    disposedValue = true;
                }
            }

            // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
            // ~HalfTagCloser() {
            //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            //   Dispose(false);
            // }

            // 添加此代码以正确实现可处置模式。
            public void Dispose()
            {
                // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
                Dispose(true);
                // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
        //写一半，后面看情况自行写入数据
        public HalfTagCloser WriteHalf(TagType tagType, string name)
        {
            //tag type
            _stream.WriteByte((byte)tagType);
            if (tagType == TagType.End) return null;
            WriteString(name);
            return new HalfTagCloser(this, tagType);
        }
        public void WriteTag(NamedBinaryTag tag)
        {
            WriteHalf(tag.Type, tag.name);
            WriteTagValue(tag);
        }
        public void WriteTagValue(NamedBinaryTag tag)
        {
            switch (tag.Type)
            {
                case TagType.End:
                    return;
                case TagType.Byte:
                    WriteByte(tag.GetByte()); return;
                case TagType.Short:
                    WriteShort(tag.GetShort()); return;
                case TagType.Int:
                    WriteInt(tag.GetInt());return;
                case TagType.Long:
                    WriteLong(tag.GetLong()); return;
                case TagType.Float:
                    WriteFloat(tag.GetFloat()); return;
                case TagType.Double:
                    WriteDouble(tag.GetDouble()); return;
                case TagType.ByteArray:
                    WriteByteArray(tag.GetByteArray()); return;
                case TagType.String:
                    WriteString(tag.GetString()); return;
                case TagType.List:
                    WriteList(tag.GetList(), tag.GetItemType()); return;
                case TagType.Compound:
                    WriteCompound(tag.GetCompound()); return;
                case TagType.IntArray:
                    WriteIntArray(tag.GetIntArray()); return;
                case TagType.LongArray:
                    WriteLongArray(tag.GetLongArray()); return;
                case TagType.None:
                    throw new Exception("tag type is None");
                default:
                    throw new Exception("unknown tag type:" + (int)tag.Type);
            }
        }
        public void WriteTagType(TagType tagType)
        {
            _stream.WriteByte((byte)tagType);
        }
        #endregion
        #region WriteFullTag
        public void WriteTag(string name, byte value)
        {
            WriteHalf(TagType.Byte, name); WriteByte(value);
        }
        public void WriteTag(string name, short value)
        {
            WriteHalf(TagType.Short, name); WriteShort(value);
        }
        public void WriteTag(string name, int value)
        {
            WriteHalf(TagType.Int, name); WriteInt(value);
        }
        public void WriteTag(string name, long value)
        {
            WriteHalf(TagType.Long, name); WriteLong(value);
        }
        public void WriteTag(string name, float value)
        {
            WriteHalf(TagType.Float, name); WriteFloat(value);
        }
        public void WriteTag(string name, double value)
        {
            WriteHalf(TagType.Double, name); WriteDouble(value);
        }
        public void WriteTag(string name, byte[] value)
        {
            WriteHalf(TagType.ByteArray, name); WriteByteArray(value);
        }
        public void WriteTag(string name, string value)
        {
            WriteHalf(TagType.String, name); WriteString(value);
        }
        public void WriteTag(string name, List<NamedBinaryTag> value)
        {
            WriteHalf(TagType.List, name);
            if (value == null || value.Count == 0)
            {
                WriteTagEnd(); WriteInt(0);
            }
            WriteList(value, value[0].Type);
        }
        public void WriteTag(string name, Dictionary<string, NamedBinaryTag> value)
        {
            WriteHalf(TagType.Compound, name); WriteCompound(value);
        }
        public void WriteTag(string name, int[] value)
        {
            WriteHalf(TagType.IntArray, name); WriteIntArray(value);
        }
        public void WriteTag(string name, long[] value)
        {
            WriteHalf(TagType.LongArray, name); WriteLongArray(value);
        }
        #endregion
        #region bigger tags
        public void WriteTagLE(string name, Vector3 v)
        {
            WriteHalf(TagType.ByteArray, name); WriteByteArrayOfVector3LE(v);
        }
        public void WriteTagLE(string name, Vector2 v)
        {
            WriteHalf(TagType.ByteArray, name); WriteByteArrayOfVector2LE(v);
        }
        public void WriteTagLE(string name, Quaternion q)
        {
            WriteHalf(TagType.ByteArray, name); WriteByteArrayOfQuaternionLE(q);
        }
        #endregion
        #region WriteValue
        BinaryWriter _leWriter;
        public void WriteFloatLE(float value)
        {
            if (this._leWriter == null)
            {
                _leWriter = new BinaryWriter(_stream);
            }
            _leWriter.Write(value);
        }
        public void WriteDoubleLE(double value)
        {
            if (this._leWriter == null)
            {
                _leWriter = new BinaryWriter(_stream);
            }
            _leWriter.Write(value);
        }
        //big-endian
        public void WriteTagEnd()
        {
            _stream.WriteByte((byte)TagType.End);
        }
        public void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }
        public void WriteShort(short value)
        {
            _buffer[1] = (byte)value;
            _buffer[0] = (byte)(value >> 8);
            _stream.Write(_buffer, 0, 2);
        }
        public void WriteInt(int value)
        {
            _buffer[3] = (byte)value;
            _buffer[2] = (byte)(value >> 8);
            _buffer[1] = (byte)(value >> 16);
            _buffer[0] = (byte)(value >> 24);
            _stream.Write(_buffer, 0, 4);
        }
        public void WriteLong(long value)
        {
            _buffer[0] = (byte)(value >> 56);
            _buffer[1] = (byte)(value >> 48);
            _buffer[2] = (byte)(value >> 40);
            _buffer[3] = (byte)(value >> 32);
            _buffer[4] = (byte)(value >> 24);
            _buffer[5] = (byte)(value >> 16);
            _buffer[6] = (byte)(value >> 8);
            _buffer[7] = (byte)(value);
            _stream.Write(_buffer, 0, 8);
        }
        MemoryStream _memoryStream;
        BinaryWriter _binaryWriter;
        public void WriteFloat(float value)
        {
            if (this._binaryWriter == null)
            {
                _memoryStream = new MemoryStream(_buffer, 0, 8, true, true); _binaryWriter = new BinaryWriter(_memoryStream);
            }
            _memoryStream.Position = 0;
            _binaryWriter.Write(value);
            Array.Reverse(_buffer, 0, 4);
            _stream.Write(_buffer, 0, 4);
        }
        public void WriteDouble(double value)
        {
            long b = BitConverter.DoubleToInt64Bits(value);
            _buffer[0] = (byte)(b >> 56);
            _buffer[1] = (byte)(b >> 48);
            _buffer[2] = (byte)(b >> 40);
            _buffer[3] = (byte)(b >> 32);
            _buffer[4] = (byte)(b >> 24);
            _buffer[5] = (byte)(b >> 16);
            _buffer[6] = (byte)(b >> 8);
            _buffer[7] = (byte)(b);
            _stream.Write(_buffer, 0, 8);
        }
        public void WriteByteArray(byte[] value)
        {
            if (value == null)
            {
                WriteInt(0); return;
            }
            WriteInt(value.Length);
            _stream.Write(value, 0, value.Length);
        }
        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteShort((short)0); return;
            }
            int requiredCount = Encoding.UTF8.GetByteCount(value);
            if (requiredCount > Int16.MaxValue)
            {
                throw new Exception("nbt string too long:" + value);
            }
            //name len
            WriteShort((short)requiredCount);
            //确保buffer够长
            if (_buffer.Length < requiredCount)
            {
                int newLen = _buffer.Length * 2;
                while (newLen < requiredCount) newLen *= 2;
                _buffer = new byte[newLen];
            }
            Encoding.UTF8.GetBytes(value, 0, value.Length, _buffer, 0);
            _stream.Write(_buffer, 0, requiredCount);
        }
        public void WriteList(List<NamedBinaryTag> value, TagType itemType)
        {
            WriteTagType(itemType);
            if (value == null)
            {
                WriteInt(0); return;
            }
            WriteInt(value.Count);
            foreach(var tag in value)
            {
                if (tag.Type != itemType) throw new Exception("type doesn't match, found type "+tag.Type+" in list of type "+itemType);
                WriteTagValue(tag);
            }
        }
        public void WriteCompound(Dictionary<string, NamedBinaryTag> value)
        {
            if (value == null)
            {
                WriteTagEnd(); return;
            }
            foreach(var pair in value){
                WriteHalf(pair.Value.Type, pair.Key);
                WriteTagValue(pair.Value);
            }
            WriteTagEnd();
        }
        public void WriteIntArray(int[] value)
        {
            if (value == null)
            {
                WriteInt(0); return;
            }
            WriteInt(value.Length);
            foreach(int v in value)
            {
                WriteInt(v);
            }
        }
        public void WriteLongArray(long[] value)
        {
            if (value == null)
            {
                WriteInt(0); return;
            }
            WriteInt(value.Length);
            foreach (long v in value)
            {
                WriteLong(v);
            }
        }
        #endregion
        #region bigger value
        public void WriteByteArrayOfVector3LE(Vector3 v)
        {
            WriteInt(12); WriteVector3LE(v);
        }
        public void WriteByteArrayOfVector2LE(Vector2 v)
        {
            WriteInt(8); WriteVector2LE(v);
        }
        public void WriteByteArrayOfQuaternionLE(Quaternion q)
        {
            WriteInt(16); WriteQuaternionLE(q);
        }
        public void WriteVector3LE(Vector3 v)
        {
            WriteFloatLE(v.x); WriteFloatLE(v.y); WriteFloatLE(v.z);
        }
        public void WriteVector2LE(Vector2 v)
        {
            WriteFloatLE(v.x); WriteFloatLE(v.y);
        }
        public void WriteQuaternionLE(Quaternion q)
        {
            WriteFloatLE(q.x); WriteFloatLE(q.y); WriteFloatLE(q.z); WriteFloatLE(q.w);
        }
        #endregion
        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._stream.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                this._stream = null;
                this._buffer = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~ByteArrayNBTReader() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

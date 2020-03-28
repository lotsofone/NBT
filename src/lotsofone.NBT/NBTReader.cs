using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

namespace lotsofone.NBT
{
    //每次读一半得到tag头。对compound和list类型不能一次读完，必须自行构造递归进行读取
    public class NBTReader : IDisposable
    {
        Stream _stream;
        byte[] _buffer = new byte[8];
        public NBTReader(Stream stream)
        {
            this._stream = stream;
        }
        public NamedBinaryTag ReadNext()
        {
            return FinishReadValue(ReadNextHalf());
        }
        #region ReadHalf
        public TagType ReadTagType()
        {
            int readLen = _stream.Read(_buffer, 0, 1);
            if (readLen <= 0) return TagType.End;
            return (TagType)_buffer[0];
        }

        // 读取下一个tag的名字和类型，并不包含数据，以便接下来决策是一次读完还是分级读还是分批读
        public NamedBinaryTag ReadNextHalf()
        {
            TagType type = ReadTagType();
            NamedBinaryTag ret = NamedBinaryTag.Create(type);
            if (ret.Type == TagType.End)
            {
                ret.name = null;
                return ret;
            }
            ret.name = ReadString();
            return ret;
        }
        //读取tag的后一半
        static readonly int[] tagValueLengths = new int[13]{0,1,2,4,8,4,8,-1,-1,-1,-1,-1,-1};
        public NamedBinaryTag FinishReadValue(NamedBinaryTag tag)
        {
            int readLen;
            switch (tag.Type)
            {
                case TagType.End:
                    return tag;
                case TagType.Byte:
                    tag.SetByte(ReadByte());
                    return tag;
                case TagType.Short:
                    tag.SetShort(ReadShort());
                    return tag;
                case TagType.Int:
                    tag.SetInt(ReadInt());
                    return tag;
                case TagType.Long:
                    tag.SetLong(ReadLong());
                    return tag;
                case TagType.Float:
                    tag.SetFloat(ReadFloat());
                    return tag;
                case TagType.Double:
                    tag.SetDouble(ReadDouble());
                    return tag;
                case TagType.ByteArray:
                    readLen = ReadInt();
                    byte[] byteArray = new byte[readLen];
                    readLen = _stream.Read(byteArray, 0, byteArray.Length);
                    if (readLen < byteArray.Length) throw new Exception("unexpected stream end");
                    tag.SetByteArray(byteArray);
                    return tag;
                case TagType.String:
                    tag.SetString(ReadString());
                    return tag;
                case TagType.List:
                    NamedBinaryTag tagItem;
                    TagType itemType = ReadTagType();
                    readLen = ReadInt();
                    List<NamedBinaryTag> list = new List<NamedBinaryTag>(readLen);
                    for (int i=0; i<readLen; i++)
                    {
                        tagItem = NamedBinaryTag.Create(itemType);
                        FinishReadValue(tagItem);
                        list.Add(tagItem);
                    }
                    tag.SetList(list);
                    return tag;
                case TagType.Compound:
                    Dictionary<string, NamedBinaryTag> compound = new Dictionary<string, NamedBinaryTag>();
                    while ((tagItem = ReadNextHalf()).Type != TagType.End)
                    {
                        FinishReadValue(tagItem);
                        compound.Add(tagItem.name, tagItem);
                    }
                    tag.SetCompound(compound);
                    return tag;
                case TagType.IntArray:
                    readLen = ReadInt();
                    int[] intArray = new int[readLen];
                    for(int i=0; i<readLen; i++)
                    {
                        intArray[i] = ReadInt();
                    }
                    tag.SetIntArray(intArray);
                    return tag;
                case TagType.LongArray:
                    readLen = ReadInt();
                    long[] longArray = new long[readLen];
                    for (int i = 0; i < readLen; i++)
                    {
                        longArray[i] = ReadLong();
                    }
                    tag.SetLongArray(longArray);
                    return tag;
                case TagType.None:
                    throw new Exception("tag type is None");
                default:
                    throw new Exception("unknown tag type:" + (int)tag.Type);
            }
        }
        //跳过tag的后一半
        public NamedBinaryTag SkipValue(NamedBinaryTag tag)
        {
            if (tag.Type == TagType.None) return tag;
            int valueLen = tagValueLengths[(int)tag.Type];
            int readLen;
            if (valueLen >= 0)
            {
                _stream.Seek(valueLen, SeekOrigin.Current);
                return tag;
            }
            TagType type;
            switch (tag.Type)
            {
                case TagType.ByteArray:
                    readLen = ReadInt();
                    _stream.Seek(readLen, SeekOrigin.Current);
                    return tag;
                case TagType.String:
                    readLen = ReadShort();
                    _stream.Seek(readLen, SeekOrigin.Current);
                    return tag;
                case TagType.List:
                    type = ReadTagType();
                    NamedBinaryTag skipTag = NamedBinaryTag.Create(type);
                    int count = ReadInt();
                    for(int i=0; i<count; i++)
                    {
                        SkipValue(skipTag);
                    }
                    return tag;
                case TagType.Compound:
                    while((type = ReadTagType()) != TagType.End)
                    {
                        skipTag = NamedBinaryTag.Create(type);
                        readLen = ReadShort();
                        _stream.Seek(readLen, SeekOrigin.Current);
                        SkipValue(skipTag);
                    }
                    return tag;
                case TagType.IntArray:
                    readLen = ReadInt();
                    _stream.Seek(readLen*4, SeekOrigin.Current);
                    return tag;
                case TagType.LongArray:
                    readLen = ReadInt();
                    _stream.Seek(readLen*8, SeekOrigin.Current);
                    return tag;
                default:
                    throw new Exception("unknown tag type:" + (int)tag.Type);
            }
        }
        #endregion
        #region DirectReadValue
        public float ReadFloatLE()
        {
            int readLen = _stream.Read(_buffer, 0, 4);
            if (readLen < 4) throw new Exception("unexpected stream end");
            if (!BitConverter.IsLittleEndian) Array.Reverse(_buffer, 0, 4);
            return BitConverter.ToSingle(_buffer, 0);
        }
        public double ReadDoubleLE()
        {
            int readLen = _stream.Read(_buffer, 0, 8);
            if (readLen < 8) throw new Exception("unexpected stream end");
            if (!BitConverter.IsLittleEndian) Array.Reverse(_buffer, 0, 8);
            return BitConverter.ToDouble(_buffer, 0);
        }
        public short ReadShort() // force big-endian
        {
            int readLen = _stream.Read(_buffer, 0, 2);
            if (readLen < 2) throw new Exception("unexpected stream end");
            if (BitConverter.IsLittleEndian) Array.Reverse(_buffer, 0, 2);
            return BitConverter.ToInt16(_buffer, 0);
        }
        public int ReadInt()
        {
            int readLen = _stream.Read(_buffer, 0, 4);
            if (readLen < 4) throw new Exception("unexpected stream end");
            if (BitConverter.IsLittleEndian) Array.Reverse(_buffer, 0, 4);
            return BitConverter.ToInt32(_buffer, 0);
        }
        public long ReadLong()
        {
            int readLen = _stream.Read(_buffer, 0, 8);
            if (readLen < 8) throw new Exception("unexpected stream end");
            if (BitConverter.IsLittleEndian) Array.Reverse(_buffer, 0, 8);
            return BitConverter.ToInt64(_buffer, 0);
        }
        public float ReadFloat()
        {
            int readLen = _stream.Read(_buffer, 0, 4);
            if (readLen < 4) throw new Exception("unexpected stream end");
            if (BitConverter.IsLittleEndian) Array.Reverse(_buffer, 0, 4);
            return BitConverter.ToSingle(_buffer, 0);
        }
        public double ReadDouble()
        {
            int readLen = _stream.Read(_buffer, 0, 8);
            if (readLen < 8) throw new Exception("unexpected stream end");
            if (BitConverter.IsLittleEndian) Array.Reverse(_buffer, 0, 8);
            return BitConverter.ToDouble(_buffer, 0);
        }
        public string ReadString()
        {
            int nameLen = ReadShort();
            if (_buffer.Length < nameLen)
            {
                int newLen = _buffer.Length * 2;
                while (newLen < nameLen) newLen *= 2;
                _buffer = new byte[newLen];
            }
            if(_stream.Read(_buffer, 0, nameLen)<nameLen)throw new Exception("unexpected stream end");
            return Encoding.UTF8.GetString(_buffer, 0, nameLen);
        }
        public byte ReadByte()
        {
            int readLen = _stream.Read(_buffer, 0, 1);
            if (readLen < 1) throw new Exception("unexpected stream end");
            return _buffer[0];
        }
        #endregion
        #region bigger value

        public Vector3 ReadByteArrayOfVector3LE()
        {
            ReadInt();
            return ReadVector3LE();
        }
        public Vector2 ReadByteArrayOfVector2LE()
        {
            ReadInt();
            return ReadVector2LE();
        }
        public Quaternion ReadByteArrayOfQuaternionLE()
        {
            ReadInt();
            return ReadQuaternionLE();
        }
        public Vector3 ReadVector3LE()
        {
            return new Vector3(ReadFloatLE(), ReadFloatLE(), ReadFloatLE());
        }
        public Vector2 ReadVector2LE()
        {
            return new Vector2(ReadFloatLE(), ReadFloatLE());
        }
        public Quaternion ReadQuaternionLE()
        {
            return new Quaternion(ReadFloatLE(), ReadFloatLE(), ReadFloatLE(), ReadFloatLE());
        }
        #endregion
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

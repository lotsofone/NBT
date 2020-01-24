using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lotsofone.NBT
{
    public abstract class NamedBinaryTag
    {
        public string name;
        public TagType Type => GetTagType();
        public abstract TagType GetTagType();

        public void RequireType(TagType type)
        {
            if (this.Type != type) throw new Exception("TagType does not match, require:" + type + " get:" + this.Type);
        }
        public static NamedBinaryTag Create(TagType type)
        {
            switch (type)
            {
                case TagType.End: return new NamedBinaryTagEnd();
                case TagType.Byte: return new NamedBinaryTagByte();
                case TagType.Short: return new NamedBinaryTagShort();
                case TagType.Int: return new NamedBinaryTagInt();
                case TagType.Long: return new NamedBinaryTagLong();
                case TagType.Float: return new NamedBinaryTagFloat();
                case TagType.Double: return new NamedBinaryTagDouble();
                case TagType.ByteArray: return new NamedBinaryTagByteArray();
                case TagType.String: return new NamedBinaryTagString();
                case TagType.List: return new NamedBinaryTagList();
                case TagType.Compound: return new NamedBinaryTagCompound();
                case TagType.IntArray: return new NamedBinaryTagIntArray();
                case TagType.LongArray: return new NamedBinaryTagLongArray();
            }
            throw new Exception("invalid tag type to create tag");
        }
        #region finishread
        #endregion
        #region getset
        public virtual byte GetByte() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetByte(byte value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual short GetShort() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetShort(short value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual int GetInt() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetInt(int value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual long GetLong() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetLong(long value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual float GetFloat() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetFloat(float value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual double GetDouble() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetDouble(double value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual byte[] GetByteArray() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetByteArray(byte[] value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual string GetString() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetString(string value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }

        public virtual TagType GetItemType() { throw new Exception("TagType does not match, this tag type:" + this.Type); }//获取List的元素类型
        public virtual void SetItemType(TagType type) { throw new Exception("TagType does not match, this tag type:" + this.Type); }//设置List的元素类型。若与数据类型不匹配则清空数据
        public virtual List<NamedBinaryTag> GetList() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetList(List<NamedBinaryTag> value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }

        public virtual Dictionary<string, NamedBinaryTag> GetCompound() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetCompound(Dictionary<string, NamedBinaryTag> value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual int[] GetIntArray() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetIntArray(int[] value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual long[] GetLongArray() { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        public virtual void SetLongArray(long[] value) { throw new Exception("TagType does not match, this tag type:" + this.Type); }
        #endregion
    }

    public class NamedBinaryTagEnd : NamedBinaryTag
    {
        public override TagType GetTagType() => TagType.End;
    }
    public class NamedBinaryTagByte : NamedBinaryTag
    {
        byte value;
        public NamedBinaryTagByte() { }
        public NamedBinaryTagByte(string name, byte value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.Byte;
        public override byte GetByte() => value;
        public override void SetByte(byte value) => this.value = value;
    }
    public class NamedBinaryTagShort : NamedBinaryTag
    {
        short value;
        public NamedBinaryTagShort() { }
        public NamedBinaryTagShort(string name, short value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.Short;
        public override short GetShort() => value;
        public override void SetShort(short value) => this.value = value;
    }
    public class NamedBinaryTagInt : NamedBinaryTag
    {
        int value;
        public NamedBinaryTagInt() { }
        public NamedBinaryTagInt(string name, int value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.Int;
        public override int GetInt() => value;
        public override void SetInt(int value) => this.value = value;
    }
    public class NamedBinaryTagLong : NamedBinaryTag
    {
        long value;
        public NamedBinaryTagLong() { }
        public NamedBinaryTagLong(string name, long value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.Long;
        public override long GetLong() => value;
        public override void SetLong(long value) => this.value = value;
    }
    public class NamedBinaryTagFloat : NamedBinaryTag
    {
        float value;
        public NamedBinaryTagFloat() { }
        public NamedBinaryTagFloat(string name, float value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.Float;
        public override float GetFloat() => value;
        public override void SetFloat(float value) => this.value = value;
    }
    public class NamedBinaryTagDouble : NamedBinaryTag
    {
        double value;
        public NamedBinaryTagDouble() { }
        public NamedBinaryTagDouble(string name, double value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.Double;
        public override double GetDouble() => value;
        public override void SetDouble(double value) => this.value = value;
    }
    public class NamedBinaryTagByteArray : NamedBinaryTag
    {
        byte[] value;
        public NamedBinaryTagByteArray() { }
        public NamedBinaryTagByteArray(string name, byte[] value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.ByteArray;
        public override byte[] GetByteArray() => value;
        public override void SetByteArray(byte[] value) => this.value = value;
    }
    public class NamedBinaryTagString : NamedBinaryTag
    {
        string value;
        public NamedBinaryTagString() { }
        public NamedBinaryTagString(string name, string value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.String;
        public override string GetString() => value;
        public override void SetString(string value) => this.value = value;
    }
    public class NamedBinaryTagList : NamedBinaryTag
    {
        TagType _itemType;
        List<NamedBinaryTag> value;
        public NamedBinaryTagList() { value = new List<NamedBinaryTag>(); }
        public NamedBinaryTagList(string name, List<NamedBinaryTag> value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.List;
        public override TagType GetItemType()
        {
            if (value == null || value.Count == 0) return _itemType;
            return value[0].Type;
        }
        public override void SetItemType(TagType type)
        {
            _itemType = type;
            for (int i = 0; i < value.Count; i++)
            {
                if (type != value[i].Type)
                {
                    value.Clear();
                    break;
                }
            }
        }

        public override List<NamedBinaryTag> GetList() => value;
        public override void SetList(List<NamedBinaryTag> value)
        {
            if (value != null && value.Count > 0)
            {
                _itemType = value[0].Type;
            }
            this.value = value;
        }
    }
    public class NamedBinaryTagCompound : NamedBinaryTag
    {
        Dictionary<string, NamedBinaryTag> value;
        public NamedBinaryTagCompound() { value = new Dictionary<string, NamedBinaryTag>(); }
        public NamedBinaryTagCompound(string name, Dictionary<string, NamedBinaryTag> value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.Compound;
        public override Dictionary<string, NamedBinaryTag> GetCompound() => value;
        public override void SetCompound(Dictionary<string, NamedBinaryTag> value) => this.value = value;
    }
    public class NamedBinaryTagIntArray : NamedBinaryTag
    {
        int[] value;
        public NamedBinaryTagIntArray() { }
        public NamedBinaryTagIntArray(string name, int[] value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.IntArray;
        public override int[] GetIntArray() => value;
        public override void SetIntArray(int[] value) => this.value = value;
    }
    public class NamedBinaryTagLongArray : NamedBinaryTag
    {
        long[] value;
        public NamedBinaryTagLongArray() { }
        public NamedBinaryTagLongArray(string name, long[] value) { this.name = name; this.value = value; }
        public override TagType GetTagType() => TagType.LongArray;
        public override long[] GetLongArray() => value;
        public override void SetLongArray(long[] value) => this.value = value;
    }
}

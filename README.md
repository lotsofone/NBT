# NBT
An optimized library to read/write named binary tag streams. Optimized for memory usage.

It is designed to allocate little memory during serialization/deserialization.

Since Bitconverter.GetBytes() always allocate a new byte[], This library never calls it;

# Usage
## NBT tree style usage
The simplist way of usage is to Deserialize a stream into an NBT tree or Serialize an NBT tree to a stream.

#### Serialize
```
NamedBinaryTag nbtTree = new NamedBinaryTagCompound();
nbtTree.name = "Andy";
var a = nbtTree.GetCompound();
NamedBinaryTag newTag;
newTag = NamedBinaryTag.Create(TagType.Int); newTag.name = "height" ; newTag.SetInt(168); a.Add(newTag.name, newTag);
newTag = NamedBinaryTag.Create(TagType.Int); newTag.name = "weight"; newTag.SetInt(60); a.Add(newTag.name, newTag);
newTag = NamedBinaryTag.Create(TagType.String); newTag.name = "nickname"; newTag.SetString("superman"); a.Add(newTag.name, newTag);
nbtTree.SetCompound(a);
Stream stream = File.Open("yourFilePath", FileMode.Open);
var writer = new NBTWriter(stream);
writer.WriteTag(nbtTree);
writer.Dispose();
```

#### Deserialize
```
Stream stream = File.Open("yourFilePath", FileMode.Open);
var reader = new NBTReader(stream);
NamedBinaryTag nbtTree = reader.ReadNext();
//And you get the nbtTree.
reader.Dispose();
```

## Reader/Writer style usage
Simply deserialize to an NBT tree will create such tree in memory, which is not effective, especially for tags of type List.

When you don't want to allocate this intermediate memory for big tags like a big compound, a long byte array or a big list, you should use this style of usage.

#### Serialize
```
Stream stream = File.Open("yourFilePath", FileMode.Open);
var writer = new NBTWriter(stream);
writer.WriteHalf(TagType.Compound, "Andy");
{
    writer.WriteHalf(TagType.Compound, "Body Data");
    writer.WriteTag("height", 168);
    writer.WriteTag("weight", 60);
    writer.WriteTagEnd();// always write a TagEnd after a compound head
    using(writer.WriteHalf(TagType.Compound, "hobbies"))
    {
        writer.WriteTag("1", "read book");
        writer.WriteTag("2", "play tennis");
        writer.WriteTag("3", "use lotsofone.NBT");
    }//or in using style, this will automatically write a TagEnd

}writer.WriteTagEnd();
writer.WriteHalf(TagType.String, "date"); writer.WriteString("2020-1-24");//WriteHalf and then write your data is always ok
writer.Dispose();
```

#### Deserialize
```
Andy andy = new Andy();
Stream stream = File.Open("yourFilePath", FileMode.Open);
var reader = new NBTReader(stream);
NamedBinaryTag tag;
reader.ReadNextHalf();//root node
while ((tag = reader.ReadNextHalf()).Type != TagType.End)
{
    switch (tag.name)
    {
        case "Body Data":
            andy.ReadBodyData(reader); //just pass down the reader, it will be loaded recursively
            continue;
        case "hobbies":
            andy.ReadHobbies(reader);
            continue;
        case "date":
            reader.FinishReadValue(tag);
            Debug.Log(tag.GetString());
            continue;
        default:
            reader.SkipValue(tag);//value if not read must be skipped
            continue;
    }

}
reader.Dispose();
```
The ReadBodyData method:
```
NamedBinaryTag tag;
while((tag = reader.ReadNext()).Type != TagType.End)
//Since we know all tags here are small types, we can just use ReadNext() instead of ReadNextHalf()
{
    switch (tag.name)
    {
        case "height":
            height = tag.GetInt(); continue;
        case "weight":
            weight = tag.GetInt(); continue;
    }
}
```
The ReadHobbies method
```
NamedBinaryTag tag;
hobbies.Clear();
while ((tag = reader.ReadNext()).Type != TagType.End)
{
    hobbies.Add(tag.GetString());
}
```

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
var a = nbtTree.GetCompound();//null is returned for new tag
a = new Dictionary<string, NamedBinaryTag>();
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

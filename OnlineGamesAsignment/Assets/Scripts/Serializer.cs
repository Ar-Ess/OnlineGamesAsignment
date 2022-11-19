using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public enum DataType
{
    INPUT_FLAG,
    WORLD_CHECK,
}
public class Serializer : MonoBehaviour
{
    public MemoryStream Serialize(uint flag) //We work with flags so we need to serialize integers
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(flag);
        return stream;
    }
    public MemoryStream SerializeFlag(uint flag) //We work with flags so we need to serialize integers
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write((int)DataType.INPUT_FLAG);
        writer.Write(flag);
        return stream;
    }

    public MemoryStream SerializeVector(Vector2 vector2) //We work with flags so we need to serialize integers
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write((int)DataType.WORLD_CHECK);
        writer.Write(vector2.x);
        writer.Write(vector2.y);
        return stream;
    }

    public uint Deserialize(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        uint rcvUint = reader.ReadUInt16();
        return rcvUint;
    }

    public uint DeserializeFlag(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(4, SeekOrigin.Begin);
        uint rcvUint = reader.ReadUInt16();
        return rcvUint;
    }

    public Vector2 DeserializeVector(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(4, SeekOrigin.Begin);
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();

        return new Vector2(x, y);
    }

    public DataType CheckDataType(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        DataType type = (DataType)BitConverter.ToInt32(reader.ReadBytes(4),0);
        
        return type;
    }
}


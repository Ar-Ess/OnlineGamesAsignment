using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public enum DataType
{
    INPUT_FLAG,
    WORLD_CHECK,
    LOBBY_MAX,
    NEXT_LEVEL
}

public static class Serializer
{
    public static byte[] Serialize(uint value, DataType type)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write((int)type);
        writer.Write(value);
        return stream.GetBuffer();
    }

    public static byte[] Serialize(DataType type)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write((int)type);
        return stream.GetBuffer();
    }

    public static byte[] Serialize(Vector2 vector2)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write((int)DataType.WORLD_CHECK);
        writer.Write(vector2.x);
        writer.Write(vector2.y);
        return stream.GetBuffer();
    }

    public class Deserialization
    {
        public Deserialization(MemoryStream stream)
        {
            this.stream = stream;
        }

        public uint Uint()
        {
            BinaryReader reader = new BinaryReader(stream);
            stream.Seek(4, SeekOrigin.Begin);
            uint rcvUint = reader.ReadUInt16();
            return rcvUint;
        }

        public Vector2 Vector2()
        {
            BinaryReader reader = new BinaryReader(stream);
            stream.Seek(4, SeekOrigin.Begin);
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();

            return new Vector2(x, y);
        }

        MemoryStream stream;
    }

    public static Deserialization Deserialize(MemoryStream stream)
    {
        return new Deserialization(stream);
    }

    public static DataType CheckDataType(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        DataType type = (DataType)BitConverter.ToInt32(reader.ReadBytes(4),0);
        
        return type;
    }
}


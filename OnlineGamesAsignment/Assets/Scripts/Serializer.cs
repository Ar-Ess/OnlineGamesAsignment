using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Serializer : MonoBehaviour
{
    public MemoryStream Serialize(uint flag) //We work with flags so we need to serialize integers
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(flag);
        return stream;
    }

    public uint Deserialize(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        uint rcvUint = reader.ReadUInt16();
        //Debug.Log("string" + rcvUint.ToString());
        return rcvUint;
    }
}


using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Serializer: MonoBehaviour
{
    private Serializer _instance;
    public Serializer instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }

        else _instance = this;
        DontDestroyOnLoad(this);
    }

    public MemoryStream Serialize(string text)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(text);
        Debug.Log("serialized");
        return stream;
    }

    public string Deserialize(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        stream.Seek(0, SeekOrigin.Begin);
        string recString = reader.ReadString();
        Debug.Log("string" + recString.ToString());
        return recString;
    }
}


using UnityEngine;
using System;

[System.Serializable]
public class ModelData {
    public float[] vertices;
    public float[] normals;
    public float[] uvs;
    public int[] faces;

    public static ModelData CreateFromJSON(string json)
    {
        ModelData instance = null;
        try
        {
            instance = JsonUtility.FromJson<ModelData>(json);
        }
        catch (Exception e)
        {
            //例外を処理する場合
        }
        return instance;
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}

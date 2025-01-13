using UnityEngine;
using System.Collections.Generic;

[SerializeField]
public enum FractalType
{
    Cube,
    Sphere,
    Pyramid,
    Octahedron
}

public struct FractalTypeData
{
    public float BevelLimit;
    public int complexityLimit;
}

public class FractalTypeUtil
{
    private static Dictionary<FractalType, FractalTypeData> FractalDataDir = new Dictionary<FractalType, FractalTypeData>(){
        {FractalType.Cube, new FractalTypeData{BevelLimit = 0.4f, complexityLimit = 6}},
        {FractalType.Sphere, new FractalTypeData{BevelLimit = 0.5f, complexityLimit = 0}},
        {FractalType.Pyramid, new FractalTypeData{BevelLimit = 0.5f, complexityLimit = 0}},
        {FractalType.Octahedron, new FractalTypeData{BevelLimit = 0.5f, complexityLimit = 0}}
    };
    public static FractalTypeData GetData(FractalType type)
    {
        return FractalDataDir[type];
    }
}
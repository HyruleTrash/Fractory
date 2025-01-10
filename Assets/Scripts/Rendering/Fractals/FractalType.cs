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
}

public class FractalTypeUtil
{
    private static Dictionary<FractalType, FractalTypeData> FractalDataDir = new Dictionary<FractalType, FractalTypeData>(){
        {FractalType.Cube, new FractalTypeData{BevelLimit = 0.5f}},
        {FractalType.Sphere, new FractalTypeData{BevelLimit = 0.5f}},
        {FractalType.Pyramid, new FractalTypeData{BevelLimit = 0.5f}},
        {FractalType.Octahedron, new FractalTypeData{BevelLimit = 0.5f}}
    };
    public static FractalTypeData GetData(FractalType type)
    {
        return FractalDataDir[type];
    }
}
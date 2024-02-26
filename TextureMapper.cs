using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[Serializable]
public struct VoxelSides
{
    public Vector2Int topMappingPos;
    public Vector2Int bottomMappingPos;
    public Vector2Int leftMappingPos;
    public Vector2Int rightMappingPos;
    public Vector2Int frontMappingPos;
    public Vector2Int backMappingPos;
}
[CreateAssetMenu(fileName = "TextureMapper", menuName = "Create TextureMapper")]
public class TextureMapper : ScriptableObject
{
    //public Dictionary<string, VoxelSides> MapperList = new Dictionary<string, VoxelSides>();
    public List<VoxelSides> voxelSides = new List<VoxelSides>();
    public List<string> textureNames = new List<string>();

    private Dictionary<string, VoxelSides> cacheDictionary; 
    
    public Dictionary<string, VoxelSides> GetDictionary()
    {
        if (voxelSides.Count != textureNames.Count)
        {
            return null;
        }

        if (cacheDictionary != null)
        {
            return cacheDictionary;
        }

        Dictionary<string, VoxelSides> result;
        //iterate through the list use for
        result = new Dictionary<string, VoxelSides>();
        for (int i = 0; i < voxelSides.Count; i++)
        {
            result.Add(textureNames[i], voxelSides[i]);
        }

        cacheDictionary = result;
        return result;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTerrain : MonoBehaviour
{
    private static MeshTerrain _instance;
    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    public ChunkProperty chunkProperty;
    public WorldProperty worldProperty;
    public TextureMapper textures;
    private Dictionary<string, VoxelSides> texturesMapping;
    private void Awake()
    {
        //worldProperty.chunkLoadRadius = worldProperty.chunkLoadRadius / 2 > 0 ? worldProperty.chunkLoadRadius : worldProperty.chunkLoadRadius + 1;
        texturesMapping = textures.GetDictionary();
        _instance = this;
    }

    public static MeshTerrain Instance()
    {
        return _instance;
    }
    
    void Start()
    {
        for (int x = 0; x < worldProperty.chunkLoadRadius; x++)
        {
            for (int z = 0; z < worldProperty.chunkLoadRadius; z++)
            {
                //var index = (x - 1) * worldProperty.chunkLoadRadius + z;
                var chunkObj = new GameObject($"Chunk{x}{z}");
                var chunk = chunkObj.AddComponent<Chunk>();
                chunk.transform.position = new Vector3(x * chunkProperty.chunkSize.x, 0, z * chunkProperty.chunkSize.z);
                chunk.SetProperty(chunkProperty, worldProperty, texturesMapping);
                var position = chunk.transform.position;
                chunks.Add( new Vector3Int((int)(position.x / chunkProperty.chunkSize.x), 0, (int)(position.z / chunkProperty.chunkSize.z)), chunk);
                chunk.Initialized();
            }
        }
        foreach (var VARIABLE in chunks)
        {
            VARIABLE.Value.GenerateMesh();
        }
    }

    public Chunk GetChunkAt(Vector3Int globalPosition)
    {
        chunks.TryGetValue(globalPosition, out var chunk);
        return chunk;
    }

    public Chunk GetChunkByVoxelGlobalPos(Vector3Int globalPos)
    {
        if (globalPos.x < 0 || globalPos.y < 0 || globalPos.z < 0)
        {
            return null;
        }
        int chunkX = globalPos.x / chunkProperty.chunkSize.x;
        int chunkY = globalPos.y / chunkProperty.chunkSize.y;
        int chunkZ = globalPos.z / chunkProperty.chunkSize.z;
        chunks.TryGetValue(new Vector3Int(chunkX, chunkY, chunkZ), out var chunk);
        return chunk;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class Chunk : MonoBehaviour
{
    private Voxel[,,] voxels;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private ChunkProperty _property;
    private WorldProperty worldProperty;

    private Dictionary<string, VoxelSides> textureMapping;
    // Define a simple Voxel struct
    public struct Voxel
    {
        public Vector3Int position;
        public Color color;
        public bool isActive; // if this voxel shows up and hide others' face
        public Voxel(Vector3Int position, Color color, bool isActive = true)
        {
            this.position = position;
            this.color = color;
            this.isActive = isActive;
        }
    }
    
    private void Awake()
    {
        // Initialize Mesh Components
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        // Call this to generate the chunk mesh
    }

    public void SetProperty( ChunkProperty property, WorldProperty worldProperty, Dictionary<string, VoxelSides> textureMapping)
    {
        this._property = property;
        this.worldProperty = worldProperty;
        this.textureMapping = textureMapping;
    }
    public void Initialized()
    {
        voxels = new Voxel[_property.chunkSize.x, _property.chunkSize.y, _property.chunkSize.z];
        InitializeVoxels();
    }
    
    
    
    public void GenerateMesh()
    {
        IterateVoxels(); // Make sure this processes all voxels

        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals(); // Important for lighting

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        // Apply a material or texture if needed
        meshRenderer.material = _property._material;
    }
    private void InitializeVoxels()
    {
        
        for (int x = 0; x < _property.chunkSize.x; x++)
        {
            for (int z = 0; z < _property.chunkSize.z; z++)
            {
                var voxelPositionXZ = Vector3Int.FloorToInt(transform.position) + new Vector3Int(x, 0, z);
                var threshold = OpenSimplex2S.Noise2(worldProperty.seed,
                    voxelPositionXZ.x * _property.chunkUnitSize / worldProperty.worldNoiseScale,
                    voxelPositionXZ.z * _property.chunkUnitSize / worldProperty.worldNoiseScale);
                threshold = ++threshold / 2; // regular [-1, 1] to [0, 1]
                threshold *= _property.chunkSize.y * worldProperty.worldNoiseHeightScale;
                
                for (int y = 0; y < _property.chunkSize.y; y++)
                {
                    var voxel = new Voxel( voxelPositionXZ + new Vector3Int(0, y, 0), Color.white);
                    //var threhold = Mathf.PerlinNoise(voxel.position.x / worldProperty.worldNoiseScale, voxel.position.z / worldProperty.worldNoiseScale);
                    //var threhold = Noise.CalcPixel2D(voxel.position.x, voxel.position.y, worldProperty.worldNoiseScale) * _property.chunkSize.y * worldProperty.worldNoiseHeightScale;

                    if (worldProperty.stage2NoiseCheck)
                    {
                        voxel.isActive = voxel.position.y < threshold;
                    }
                    voxels[x, y, z] = voxel;
                }
            }
        }
    }
    
    private void IterateVoxels()
    {
        for (int y = 0; y < _property.chunkSize.y; y++)
        {
            for (int z = 0; z < _property.chunkSize.z; z++)
            {
                for (int x = 0; x < _property.chunkSize.x; x++)
                {
                    ProcessVoxel(x, y, z);
                }
            }
        }
    }
    
    private void ProcessVoxel(int x, int y, int z)
    {
        // Check if the voxels array is initialized and the indices are within bounds
        if (voxels == null || x < 0 || x >= voxels.GetLength(0) || 
            y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
        {
            return; // Skip processing if the array is not initialized or indices are out of bounds
        }
        Voxel voxel = voxels[x, y, z];
        if (voxel.isActive)
        {
            // Check each face of the voxel for visibility
            bool[] facesVisible = new bool[6];

            // Check visibility for each face
            facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
            facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
            facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
            facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
            facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
            facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back
        
            for (int i = 0; i < facesVisible.Length; i++)
            {
                if (facesVisible[i])
                    AddFaceData(x, y, z, i); // Method to add mesh data for the visible face
            }
        }
    }
    
    private bool IsFaceVisible(int x, int y, int z)
    {
        // Convert local chunk coordinates to global coordinates
        Vector3Int globalPos = LocalVoxelPosToWorldPos(new Vector3Int(x, y, z));
        Voxel? voxel = GetVoxel(globalPos);
        return !worldProperty.stage1VisibleCheck || voxel == null || !voxel.Value.isActive;
    }

    public static Voxel? GetVoxel(Vector3Int globalPos)
    {
        Chunk chunk = MeshTerrain.Instance().GetChunkByVoxelGlobalPos(globalPos);
        if (chunk == null)
        {
            return null;
        }
        Vector3Int localVoxelPos = chunk.WorldVoxelPosToLocalPos(globalPos);
        try
        {
            return chunk.voxels[localVoxelPos.x, localVoxelPos.y, localVoxelPos.z];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private void AddFaceData(int x, int y, int z, int faceIndex)
    {
    // Based on faceIndex, determine vertices and triangles
    // Add vertices and triangles for the visible face
    // Calculate and add corresponding UVs
    VoxelSides sidesInfo = textureMapping["grass"];
    
    if (faceIndex == 0) // Top Face
    {
        var side = sidesInfo.topMappingPos;
        vertices.Add(new Vector3(x,     y + 1, z    ));
        vertices.Add(new Vector3(x,     y + 1, z + 1)); 
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        vertices.Add(new Vector3(x + 1, y + 1, z    ));
        /*
        var picIndex = new Vector2Int(3, 1);
        uvs.Add(new Vector2((picIndex.x + 1) * (float)0.03125, picIndex.y * (float)0.03125));
        uvs.Add(new Vector2(picIndex.x * (float)0.03125, picIndex.y * (float)0.03125));
        uvs.Add(new Vector2(picIndex.x * (float)0.03125, (picIndex.y + 1) * (float)0.03125));
        uvs.Add(new Vector2((picIndex.x + 1) * (float)0.03125, (picIndex.y + 1) * (float)0.03125));
        */
        
        uvs.Add(new Vector2(side.x, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y + 1) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y + 1) * (float)0.03125);
    }
    
    if (faceIndex == 1) // Bottom Face
    {
        var side = sidesInfo.bottomMappingPos;
        vertices.Add(new Vector3(x,     y, z    ));
        vertices.Add(new Vector3(x + 1, y, z    )); 
        vertices.Add(new Vector3(x + 1, y, z + 1));
        vertices.Add(new Vector3(x,     y, z + 1)); 
        uvs.Add(new Vector2(side.x, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y + 1) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y + 1) * (float)0.03125);
    }

    if (faceIndex == 2) // Left Face
    {
        var side = sidesInfo.bottomMappingPos;
        vertices.Add(new Vector3(x, y,     z    ));
        vertices.Add(new Vector3(x, y,     z + 1));
        vertices.Add(new Vector3(x, y + 1, z + 1));
        vertices.Add(new Vector3(x, y + 1, z    ));
        uvs.Add(new Vector2(side.x + 1, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y + 1) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y + 1) * (float)0.03125);
    }

    if (faceIndex == 3) // Right Face
    {
        var side = sidesInfo.rightMappingPos;
        vertices.Add(new Vector3(x + 1, y,     z + 1));
        vertices.Add(new Vector3(x + 1, y,     z    ));
        vertices.Add(new Vector3(x + 1, y + 1, z    ));
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        uvs.Add(new Vector2(side.x, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y + 1) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y + 1) * (float)0.03125);
    }

    if (faceIndex == 4) // Front Face
    {
        var side = sidesInfo.frontMappingPos;
        vertices.Add(new Vector3(x,     y,     z + 1));
        vertices.Add(new Vector3(x + 1, y,     z + 1));
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        vertices.Add(new Vector3(x,     y + 1, z + 1));
        uvs.Add(new Vector2(side.x + 1, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y + 1) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y + 1) * (float)0.03125);
    }

    if (faceIndex == 5) // Back Face
    {
        var side = sidesInfo.backMappingPos;
        vertices.Add(new Vector3(x + 1, y,     z    ));
        vertices.Add(new Vector3(x,     y,     z    ));
        vertices.Add(new Vector3(x,     y + 1, z    ));
        vertices.Add(new Vector3(x + 1, y + 1, z    ));
        uvs.Add(new Vector2(side.x, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y) * (float)0.03125);
        uvs.Add(new Vector2(side.x + 1, side.y + 1) * (float)0.03125);
        uvs.Add(new Vector2(side.x, side.y + 1) * (float)0.03125);
        
    }
    AddTriangleIndices();
    }
    private void AddTriangleIndices()
    {
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
    }
    private Vector3Int WorldVoxelPosToLocalPos(Vector3Int targetWorldVoxelPos)
    {
        //De-Offset
        return targetWorldVoxelPos - Vector3Int.FloorToInt(transform.position);
    }

    private Vector3Int LocalVoxelPosToWorldPos(Vector3Int localVoxelPos)
    {
        return Vector3Int.FloorToInt(transform.position) + localVoxelPos;
    }
}
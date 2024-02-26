using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTester : MonoBehaviour
{
    public Vector2 uv1 = new Vector2();
    public Vector2 uv2 = new Vector2();
    public Vector2 uv3 = new Vector2();
    public Vector2 uv4 = new Vector2();
    public Vector2Int picIndex = new Vector2Int();
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    // Start is called before the first frame update
    private Mesh mesh;
    void Start()
    {   
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        
        var x = 0;
        var y = 0;
        var z = 0;
        
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        
        vertices.Add(new Vector3(x,     y + 1, z    ));
        vertices.Add(new Vector3(x,     y + 1, z + 1)); 
        vertices.Add(new Vector3(x + 1, y + 1, z + 1));
        vertices.Add(new Vector3(x + 1, y + 1, z    )); 
        
        int vertCount = vertices.Count;

        // First triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 3);
        triangles.Add(vertCount - 2);

        // Second triangle
        triangles.Add(vertCount - 4);
        triangles.Add(vertCount - 2);
        triangles.Add(vertCount - 1);
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        uv1 = (new Vector2((picIndex.x + 1) * (float)0.03125, picIndex.y * (float)0.03125));
        uv2 = (new Vector2(picIndex.x * (float)0.03125, picIndex.y * (float)0.03125));
        uv3 = (new Vector2(picIndex.x * (float)0.03125, (picIndex.y + 1) * (float)0.03125));
        uv4 = (new Vector2((picIndex.x + 1) * (float)0.03125, (picIndex.y + 1) * (float)0.03125));
        uvs = new List<Vector2>()
        {
            uv1, uv2, uv3, uv4
        };
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
    }
}

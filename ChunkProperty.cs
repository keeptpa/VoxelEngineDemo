using UnityEngine;

[CreateAssetMenu(fileName = "ChunkProperty", menuName = "Create ChunkProperty")]
public class ChunkProperty : ScriptableObject
{
    public Material _material;
    public Vector3Int chunkSize = new Vector3Int(16, 16, 16);
    public float chunkUnitSize = 1f;
}
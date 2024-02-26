using UnityEngine;

[CreateAssetMenu(fileName = "WorldProperty", menuName = "Create WorldProperty")]
public class WorldProperty : ScriptableObject
{
    public float worldNoiseScale = 1f;
    public float worldNoiseHeightScale = 1f;
    public int chunkLoadRadius = 3;
    public int seed = 1919810;

    public bool stage1VisibleCheck = false; //Check this to turn on face culling
    public bool stage2NoiseCheck = false; //Check this to turn on noise 
}
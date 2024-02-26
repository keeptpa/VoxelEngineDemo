using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct BlockTextures
{
    public Texture TopTexture;
    public Texture BottomTexture;
    public Texture LeftTexture;
    public Texture RightTexture;
    public Texture ForwardTexture;
    public Texture BackTexture;
    public string id;
}

[CreateAssetMenu(fileName = "TexturePacker", menuName = "Create TexturePacker")]
public class TexturePacker : ScriptableObject
{
    public List<BlockTextures> packList =  new List<BlockTextures>();
}

[CustomEditor(typeof(TexturePacker))]
public class TestScriptableEditor : Editor
{
    private Vector3 pointer = Vector3.zero;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (TexturePacker)target;
        if(GUILayout.Button("Add to Counter", GUILayout.Height(40)))
        {
            var path = AssetDatabase.GetAssetPath(target.GetInstanceID());
            foreach (var VARIABLE in script.packList)
            {
            }
        }
    }
}
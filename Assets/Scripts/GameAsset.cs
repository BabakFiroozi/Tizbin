using UnityEngine;
using UnityEditor;

public class GameAsset : ScriptableObject
{
    public Sprite AllIcons = null;

    [MenuItem("Tools/Game Asset")]
    public static void CreateGameAssets()
    {
        var obj = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/GameAsset.asset");
        Selection.activeObject = obj;
    }
}
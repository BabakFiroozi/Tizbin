using System;
using UnityEngine;

public class GameAsset : ScriptableObject
{
    [Serializable]
    public class WorldTimeAPIInfo
    {
        public string url;
        public string field;
    }
    
    [SerializeField] WorldTimeAPIInfo _worldTimeAPI;
    
    [SerializeField] Sprite[] _allSprites = null;

    public WorldTimeAPIInfo WorldTimeAPI => _worldTimeAPI;
    
    public Sprite[] AllSprites => _allSprites;

    static GameAsset _instance;

    public static GameAsset Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameAsset>("GameAsset");
            }
            return _instance;
        }
    }

    public bool DailyIsLocal { get; set; }
}
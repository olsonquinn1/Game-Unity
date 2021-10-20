using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
#if UNITY_EDITPR
using UnityEditor;
#endif

public class Wall_Tile : Tile
{
    public Sprite[] m_Sprites;
    public Sprite m_Preview;

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        int mask = 0;
        for (int sel = 0; sel < 4; sel++)
        {
            mask *= 2;
            Vector3Int position = new Vector3Int(location.x + 3 * ((sel == 2)?1:0) - 3 * ((sel == 0)?1:0), location.y + 3 * ((sel == 3)?1:0) - 3 * ((sel == 1)?1:0), location.z);
            mask += HasWall(tilemap, position) ? 1 : 0;
        }
        if (mask >= 0 && mask < m_Sprites.Length)
        {
            tileData.sprite = m_Sprites[mask];
            tileData.color = Color.white;
            var m = tileData.transform;
            Quaternion q = new Quaternion();//may need tweaking
            m.SetTRS(Vector3.zero, q, Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.None;
        }
        else
        {
            Debug.LogWarning("Not enough sprites in WallTile instance");
        }
    }

    private bool HasWall(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if UNITY_EDITOR
// The following is a helper that adds a menu item to create a RoadTile Asset
    /*[MenuItem("Assets/Create/RoadTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Road Tile", "New Road Tile", "Asset", "Save Road Tile", "Assets");
        if (path == "")
            return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoadTile>(), path);
    }*/
#endif
}

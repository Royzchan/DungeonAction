using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public Transform mapGridParent; // UIのGridの親オブジェクト (Hierarchy上の空のオブジェクトを指定)
    public GameObject wallTilePrefab;   // 壁タイルのプレハブ
    public GameObject floorTilePrefab;  // 床タイルのプレハブ
    public GameObject startTilePrefab;  // スタート部屋タイルのプレハブ
    public GameObject bossTilePrefab;   // ボス部屋タイルのプレハブ
    public GameObject playerIconPrefab; // プレイヤーアイコンのプレハブ

    private GameObject[,] mapTiles;     // タイルのGameObject配列
    private int[,] dungeonMap;          // ダンジョンマップのデータ
    private Vector2Int mapSize;         // マップ全体のサイズ (width, height)

    private GameObject playerIcon;      // プレイヤーを表すアイコン
    private float tileSize = 20f;       // UI上の1マスのサイズ (ピクセル)

    private Vector2 playerPosition;     // プレイヤーの現在位置 (詳細な位置指定が可能)

    /// <summary>
    /// マップを初期化して生成する
    /// </summary>
    /// <param name="dungeonMap">ダンジョンマップのデータ</param>
    public void InitializeMap(int[,] dungeonMap)
    {
        this.dungeonMap = dungeonMap;
        mapSize = new Vector2Int(dungeonMap.GetLength(0), dungeonMap.GetLength(1));

        // タイルオブジェクト配列を初期化
        mapTiles = new GameObject[mapSize.x, mapSize.y];

        // GridにUIタイルを動的生成
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // タイルPrefabを取得
                GameObject tilePrefab = GetTilePrefab(dungeonMap[x, y]);
                GameObject tile = Instantiate(tilePrefab, mapGridParent);

                // タイルのサイズと位置を設定
                RectTransform rt = tile.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(tileSize, tileSize);
                rt.anchoredPosition = new Vector2((x - mapSize.x / 2f) * tileSize, (y - mapSize.y / 2f) * tileSize);

                mapTiles[x, y] = tile; // タイルを配列に保存
            }
        }

        // プレイヤーアイコンを生成
        playerIcon = Instantiate(playerIconPrefab, mapGridParent);
        RectTransform playerRect = playerIcon.GetComponent<RectTransform>();
        playerRect.sizeDelta = new Vector2(tileSize, tileSize);

        // 親の中心位置調整
        CenterMapGrid();

        // 初期プレイヤー位置を設定
        playerPosition = Vector2.zero;
        UpdatePlayerPosition(playerPosition, Vector2.up);
    }

    /// <summary>
    /// Grid全体の親オブジェクトを画面の中心に配置
    /// </summary>
    private void CenterMapGrid()
    {
        RectTransform gridParentRect = mapGridParent.GetComponent<RectTransform>();
        gridParentRect.anchoredPosition = Vector2.zero; // 親が常に画面中心に配置される
    }

    /// <summary>
    /// マップのタイルPrefabを取得
    /// </summary>
    /// <param name="tileType">タイルタイプ (0:壁, 1:床, 2:スタート部屋, 3:ボス部屋)</param>
    /// <returns>対応するPrefab</returns>
    private GameObject GetTilePrefab(int tileType)
    {
        switch (tileType)
        {
            case 1: return floorTilePrefab; // 床
            case 2: return startTilePrefab; // スタート部屋
            case 3: return bossTilePrefab;  // ボス部屋
            default: return wallTilePrefab; // 壁
        }
    }

    /// <summary>
    /// プレイヤーの現在位置と向きを更新
    /// </summary>
    /// <param name="newPosition">プレイヤーの新しい位置 (詳細な座標指定)</param>
    /// <param name="facingDirection">プレイヤーが向いている方向 (例: 上 (0,1))</param>
    public void UpdatePlayerPosition(Vector2 newPosition, Vector2 facingDirection)
    {
        playerPosition = newPosition;

        // プレイヤーアイコンの位置を更新 (0,0 を中心とした位置計算)
        RectTransform playerRect = playerIcon.GetComponent<RectTransform>();
        playerRect.anchoredPosition = new Vector2(playerPosition.x * tileSize, playerPosition.y * tileSize);

        // プレイヤーアイコンの向きを更新
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
        playerRect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// プレイヤーの現在座標を取得
    /// </summary>
    /// <returns>現在の座標 (詳細な位置)</returns>
    public Vector2 GetPlayerPosition()
    {
        return playerPosition;
    }
}

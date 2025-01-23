using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    private PlayerController _player;
    public int width = 50;   // マップ全体の横幅
    public int height = 50;  // マップ全体の高さ
    public int roomCount = 10; // 一般部屋の数
    public Vector2Int roomMinSize = new Vector2Int(3, 3);
    public Vector2Int roomMaxSize = new Vector2Int(10, 10);

    public GameObject floorPrefab;       // 一般部屋の床プレハブ
    public GameObject wallPrefab;        // 壁のプレハブ
    public GameObject startRoomPrefab;   // スタート部屋の床プレハブ
    public GameObject bossRoomPrefab;    // ボス部屋の床プレハブ

    public int maxCorridorLength = 10; // 通路の最大長さ

    private int[,] dungeonMap; // 0 = 壁, 1 = 一般床, 2 = スタート床, 3 = ボス床
    private List<Rect> rooms = new List<Rect>();
    private Rect startRoom;
    private Rect bossRoom;
    private bool bossRoomCreated = false;

    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        GenerateDungeon();
        InstantiateDungeon();
        PlacePlayer();
    }

    void GenerateDungeon()
    {
        dungeonMap = new int[width, height];

        // すべて壁で初期化
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonMap[x, y] = 0;
            }
        }

        // 1. スタート部屋を作成
        startRoom = CreateRandomRoom(2); // 特殊フラグ 2 (スタート部屋)
        rooms.Add(startRoom);

        // 2. 一般部屋を追加
        for (int i = 0; i < roomCount; i++)
        {
            Rect newRoom = CreateRandomRoom(1); // 一般部屋フラグ 1
            if (newRoom.width > 0 && newRoom.height > 0)
            {
                rooms.Add(newRoom);
            }
        }

        // 3. ボス部屋を作成 (1回のみ生成されるように制限)
        bossRoom = CreateBossRoom();
        if (bossRoom.width > 0 && bossRoom.height > 0)
        {
            rooms.Add(bossRoom);
        }

        // 4. 部屋をつなぐ通路を生成
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Vector2Int currentRoomCenter = GetRoomCenter(rooms[i]);
            Vector2Int nextRoomCenter = GetRoomCenter(rooms[i + 1]);
            CreateLimitedCorridor(currentRoomCenter, nextRoomCenter);
        }
    }

    Rect CreateRandomRoom(int roomType)
    {
        int roomWidth = Random.Range(roomMinSize.x, roomMaxSize.x);
        int roomHeight = Random.Range(roomMinSize.y, roomMaxSize.y);
        int x = Random.Range(1, width - roomWidth - 2); // 外壁用に1マス余裕を持たせる
        int y = Random.Range(1, height - roomHeight - 2);

        Rect newRoom = new Rect(x, y, roomWidth, roomHeight);

        // 部屋が他の部屋と重ならない場合のみ追加
        if (!IsOverlapping(newRoom))
        {
            MarkRoomOnMap(newRoom, roomType);
            return newRoom;
        }
        return new Rect(0, 0, 0, 0);
    }

    Rect CreateBossRoom()
    {
        if (bossRoomCreated) return new Rect(0, 0, 0, 0); // すでにボス部屋が生成されている場合は作成しない

        Vector2Int startCenter = GetRoomCenter(startRoom);
        float maxDistance = 0;
        Rect bestRoom = new Rect(0, 0, 0, 0);

        // スタート部屋から最も遠い場所を探してボス部屋を配置
        for (int attempt = 0; attempt < 20; attempt++)
        {
            int roomWidth = Random.Range(roomMinSize.x, roomMaxSize.x);
            int roomHeight = Random.Range(roomMinSize.y, roomMaxSize.y);
            int x = Random.Range(1, width - roomWidth - 2); // 外壁用に1マス余裕を持たせる
            int y = Random.Range(1, height - roomHeight - 2);
            Rect potentialRoom = new Rect(x, y, roomWidth, roomHeight);

            if (!IsOverlapping(potentialRoom))
            {
                Vector2Int potentialCenter = GetRoomCenter(potentialRoom);
                float distance = Vector2.Distance(startCenter, potentialCenter);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    bestRoom = potentialRoom;
                }
            }
        }

        if (bestRoom.width > 0 && bestRoom.height > 0)
        {
            MarkRoomOnMap(bestRoom, 3); // 特殊フラグ 3 (ボス部屋)
            bossRoomCreated = true;
        }
        return bestRoom;
    }

    void MarkRoomOnMap(Rect room, int roomType)
    {
        for (int i = (int)room.xMin - 1; i <= (int)room.xMax; i++)
        {
            for (int j = (int)room.yMin - 1; j <= (int)room.yMax; j++)
            {
                // マップの範囲外チェック
                if (i >= 0 && i < width && j >= 0 && j < height)
                {
                    if (i >= room.xMin && i < room.xMax && j >= room.yMin && j < room.yMax)
                    {
                        dungeonMap[i, j] = roomType; // 部屋の内部をマーク
                    }
                }
            }
        }
    }

    bool IsOverlapping(Rect newRoom)
    {
        Rect expandedRoom = new Rect(newRoom.xMin - 1, newRoom.yMin - 1, newRoom.width + 2, newRoom.height + 2);
        foreach (Rect room in rooms)
        {
            if (expandedRoom.Overlaps(room))
                return true;
        }
        return false;
    }

    Vector2Int GetRoomCenter(Rect room)
    {
        return new Vector2Int((int)(room.xMin + room.width / 2), (int)(room.yMin + room.height / 2));
    }

    void CreateLimitedCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        // 確実に部屋をつなげるために、途中に中間点を設定
        while (Vector2.Distance(current, end) > maxCorridorLength)
        {
            Vector2Int midpoint = new Vector2Int(
                (current.x + end.x) / 2,
                (current.y + end.y) / 2
            );

            CreateDirectCorridor(current, midpoint);
            current = midpoint;
        }

        // 最後に残った部分を直接つなげる
        CreateDirectCorridor(current, end);
    }

    void CreateDirectCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        // X軸方向に移動
        while (current.x != end.x)
        {
            dungeonMap[current.x, current.y] = 1;
            current.x += (end.x > current.x) ? 1 : -1;
        }

        // Y軸方向に移動
        while (current.y != end.y)
        {
            dungeonMap[current.x, current.y] = 1;
            current.y += (end.y > current.y) ? 1 : -1;
        }
    }

    void InstantiateDungeon()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                switch (dungeonMap[x, y])
                {
                    case 1:
                        Instantiate(floorPrefab, position * 4, Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(startRoomPrefab, position * 4, Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(bossRoomPrefab, position * 4, Quaternion.identity);
                        break;
                    default:
                        Instantiate(wallPrefab, position * 4, Quaternion.identity);
                        break;
                }
            }
        }
    }

    void PlacePlayer()
    {
        if (_player != null)
        {
            Vector2Int startCenter = GetRoomCenter(startRoom);
            Vector3 playerPosition = new Vector3(startCenter.x * 4, 0, startCenter.y * 4);
            _player.SetPos(playerPosition);
        }
    }
}

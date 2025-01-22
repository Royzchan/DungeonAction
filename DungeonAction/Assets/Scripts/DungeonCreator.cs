using System.Collections.Generic;
using UnityEngine;

public class RoomAndCorridorGenerator : MonoBehaviour
{
    public int width = 50;   // マップ全体の横幅
    public int height = 50;  // マップ全体の高さ
    public int roomCount = 10; // 一般部屋の数
    public Vector2Int roomMinSize = new Vector2Int(3, 3);
    public Vector2Int roomMaxSize = new Vector2Int(10, 10);

    public GameObject floorPrefab;       // 一般部屋の床プレハブ
    public GameObject wallPrefab;        // 壁のプレハブ
    public GameObject startRoomPrefab;   // スタート部屋の床プレハブ
    public GameObject bossRoomPrefab;    // ボス部屋の床プレハブ

    private int[,] dungeonMap; // 0 = 壁, 1 = 一般床, 2 = スタート床, 3 = ボス床
    private List<Rect> rooms = new List<Rect>();
    private Rect startRoom;
    private Rect bossRoom;
    private bool bossRoomCreated = false;

    void Start()
    {
        GenerateDungeon();
        InstantiateDungeon();
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
            CreateCorridor(currentRoomCenter, nextRoomCenter);
        }
    }

    Rect CreateRandomRoom(int roomType)
    {
        int roomWidth = Random.Range(roomMinSize.x, roomMaxSize.x);
        int roomHeight = Random.Range(roomMinSize.y, roomMaxSize.y);
        int x = Random.Range(1, width - roomWidth - 1);
        int y = Random.Range(1, height - roomHeight - 1);

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
            int x = Random.Range(1, width - roomWidth - 1);
            int y = Random.Range(1, height - roomHeight - 1);
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
        for (int i = (int)room.xMin; i < (int)room.xMax; i++)
        {
            for (int j = (int)room.yMin; j < (int)room.yMax; j++)
            {
                dungeonMap[i, j] = roomType; // 特定の部屋種別をマップにマーク
            }
        }
    }

    bool IsOverlapping(Rect newRoom)
    {
        foreach (Rect room in rooms)
        {
            if (newRoom.Overlaps(room))
                return true;
        }
        return false;
    }

    Vector2Int GetRoomCenter(Rect room)
    {
        return new Vector2Int((int)(room.xMin + room.width / 2), (int)(room.yMin + room.height / 2));
    }

    void CreateCorridor(Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;

        while (current.x != end.x)
        {
            dungeonMap[current.x, current.y] = 1; // 通路は一般床扱い
            current.x += (end.x > current.x) ? 1 : -1;
        }

        while (current.y != end.y)
        {
            dungeonMap[current.x, current.y] = 1; // 通路は一般床扱い
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
}

using Fusion;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap groundTilemap; // Tham chiếu đến thành phần Tilemap trong "Ground"
    public int seed; // Seed để lưu lại và tái tạo map
    public TileBase[] tileset; // Mảng chứa các gạch trong tileset
    public TileBase specialTile; // Tileset riêng cho hàng cuối cùng của mỗi cột
    public GameObject portal;
    public int mapWidth; // Chiều rộng của bản đồ
    public int mapHeight; // Chiều cao của bản đồ
    public GameObject[] objectPrefabs;
    public float noiseScale; // Độ phân giải của noise map
    public int octaves; // Số lượng octaves trong noise map
    public float persistence; // Độ ảnh hưởng của octave cao hơn đến noise map
    public float lacunarity; // Mức độ gia tăng tần số của octave cao hơn trong noise map
    public Vector2 offset; // Điều chỉnh tọa độ của noise map
    [SerializeField]
    private float increasePercentage = 30f; // Tỉ lệ tăng (30%)
    [SerializeField]
    private float decreasePercentage = 20f; // Tỉ lệ giảm (20%)
    [SerializeField]
    private int minDistancePlaceObject = 1;
    [SerializeField]
    private int maxDistancePlaceObject = 5;
    [SerializeField]
    private NetworkRunner networkRunner;
    private bool isMapGenerated = false;
    private void Start()
    {
        

    }
    private void Update()
    {
        
    }

    // Trong lớp MapGenerator
    private Vector3Int npcSpawnPosition; // Lưu trữ vị trí của NPC

    private void PlaceNPCOnSpecialTile()
    {
        // Tạo một vị trí ngẫu nhiên trên specialTile
        int x = UnityEngine.Random.Range(0, mapWidth);
        int y = mapHeight - 1; // Vị trí y của specialTile
        npcSpawnPosition = new Vector3Int(x, y, 0);

        // Đặt NPC Controller tại vị trí này
        if (portal != null)
        {
            GameObject npc = Instantiate(portal, groundTilemap.CellToWorld(npcSpawnPosition), Quaternion.identity);
        }
        // Get the UIShop reference and assign it to the NPC's NPCsController component
        //UIShop uiShop = FindObjectOfType<UIShop>();
        //NPCsController npcController = npc.GetComponent<NPCsController>();
        //if (npcController != null && uiShop != null)
        //{
        //    npcController.uiShop = uiShop;
        //}
    }
    public void GenerateMapWithSeed(int mapSeed)
    {
        // Lưu lại seed mới
        seed = mapSeed;

        // Tiếp tục quá trình tạo map bằng cách sử dụng seed mới
        UnityEngine.Random.InitState(mapSeed);
        GenerateMap();
    }
    public void SetSeed(int newSeed)
    {
        seed = newSeed;
    }
    public void SaveMapSeed()
    {
        MapDataSaver.SaveSeed(seed);
    }
    public MapData SaveMapData()
    {
        MapData mapData = new MapData();
        mapData.seed = seed;
        mapData.tileset = tileset;
        mapData.specialTile = specialTile;
        mapData.objectPrefabs = objectPrefabs;
        return mapData;
    }
    public void GenerateMap()
    {
        //int seed = MapDataSaver.LoadHostSeed();
        //UnityEngine.Random.InitState(seed);
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, offset);

        TileBase emptyTile = null; // Empty tile for empty cells

        int previousColumnTileCount = mapHeight - UnityEngine.Random.Range(1, 4); // Number of tiles in the first column

        int maxEmptyTileCount = 5; // Maximum number of empty cells in each column

        int limit = mapHeight - maxEmptyTileCount; // Maximum number of rows

        bool reachedLimit = false; // Boolean variable to check if the limit has been reached

        int objectSpawnCount = 0; // Counter for the number of specialTiles processed

        for (int x = 0; x < mapWidth; x++)
        {
            int currentColumnTileCount = previousColumnTileCount; // Default number of tiles in the column is the same as the previous column

            if (!reachedLimit)
            {
                float randomValue = UnityEngine.Random.Range(0f, 100f); // Tạo giá trị ngẫu nhiên từ 0 đến 100

                if (randomValue < increasePercentage)
                {
                    currentColumnTileCount++; // Increase the number of tiles in the column
                }
                else if (randomValue < increasePercentage + decreasePercentage)
                {
                    currentColumnTileCount--; // Decrease the number of tiles in the column
                }

                currentColumnTileCount = Mathf.Clamp(currentColumnTileCount, 1, mapHeight - 1); // Limit the range to 1 to mapHeight - 1

                int emptyTileCount = mapHeight - currentColumnTileCount;

                if (emptyTileCount > maxEmptyTileCount)
                {
                    emptyTileCount = Mathf.Min(emptyTileCount, previousColumnTileCount); // Limit the emptyTileCount to the number of rows in the previous column
                    currentColumnTileCount = mapHeight - emptyTileCount; // Update the number of tiles in the column

                    if (currentColumnTileCount >= limit)
                    {
                        currentColumnTileCount = previousColumnTileCount; // Keep the same number of rows
                        emptyTileCount = mapHeight - currentColumnTileCount;
                        reachedLimit = true; // Reached the limit
                    }
                }
            }

            for (int y = 0; y < mapHeight; y++)
            {
                float currentHeight = noiseMap[x, y];

                // Check if the current row is the last non-empty row
                if (y == currentColumnTileCount - 1)
                {
                    // Set specialTile
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    groundTilemap.SetTile(tilePosition, specialTile);
                    objectSpawnCount++; // Increase the counter

                    // Check if the counter has reached the desired value to spawn an object
                    if (objectSpawnCount >= UnityEngine.Random.Range(minDistancePlaceObject, maxDistancePlaceObject))
                    {
                        // Spawn random object on top of specialTile
                        int randomIndex = UnityEngine.Random.Range(0, objectPrefabs.Length);
                        GameObject objectPrefab = objectPrefabs[randomIndex];
                        Vector3 spawnPosition = groundTilemap.CellToWorld(tilePosition) + new Vector3(0f, .5f, 0f); // Adjust spawn position to be on top of the tile
                        Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

                        objectSpawnCount = 0; // Reset the counter
                    }
                }
                else if (y >= mapHeight - (mapHeight - currentColumnTileCount))
                {
                    // Set emptyTile
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    groundTilemap.SetTile(tilePosition, emptyTile);
                }
                else
                {
                    // Set tile based on current height
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    groundTilemap.SetTile(tilePosition, ChooseTile(currentHeight));
                }
            }

            previousColumnTileCount = currentColumnTileCount;
        }
    }
    private TileBase ChooseTile(float height)
    {
        // Lựa chọn tile dựa trên chiều cao
        if (height < 0.2f)
        {
            return tileset[0]; // Tile 0 là loại gạch thấp nhất
        }
        else if (height < 0.4f)
        {
            return tileset[1]; // Tile 1 là loại gạch thấp hơn
        }
        else if (height < 0.6f)
        {
            return tileset[2]; // Tile 2 là loại gạch trung bình
        }
        else if (height < 0.8f)
        {
            return tileset[3]; // Tile 3 là loại gạch cao hơn
        }
        else
        {
            return tileset[4]; // Tile 4 là loại gạch cao nhất
        }
    }
}
[System.Serializable]
public class MapData
{
    public int seed;
    public TileBase[] tileset;
    public TileBase specialTile;
    public GameObject[] objectPrefabs;
    public Vector3 npcPosition; // Thêm thuộc tính để lưu trữ vị trí của npcPrefab
}
public static class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + offset.x) / scale * frequency;
                    float sampleY = (y - halfHeight + offset.y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize the noise map to a range between 0 and 1
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
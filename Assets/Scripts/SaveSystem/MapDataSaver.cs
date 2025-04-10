using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MapDataSaver
{
    public static string directoryPath = @"C:\SaveLoadTest"; // Đường dẫn tới thư mục lưu trữ
    public static string filePath = Path.Combine(directoryPath, "seed.json"); // Đường dẫn tới tệp seed
    public static string hostSeedFilePath = Path.Combine(directoryPath, "hostSeed.json"); // Đường dẫn tới tệp seed của người host

    // Kiểm tra và tạo thư mục nếu chưa tồn tại
    private static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("Directory created: " + directoryPath);
        }
    }

    public static void SaveSeed(int seed)
    {
        EnsureDirectoryExists();
        SeedData seedData = new SeedData { seed = seed };
        string json = JsonUtility.ToJson(seedData);
        try
        {
            // Ghi chuỗi JSON vào tệp seed của người host
            File.WriteAllText(filePath, json);
            Debug.Log("Seed saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving seed: " + e.Message);
        }
    }

    // Save the host seed to a separate file
    public static void SaveHostSeed(int hostSeed)
    {
        EnsureDirectoryExists();
        SeedData seedData = new SeedData { seed = hostSeed };
        string json = JsonUtility.ToJson(seedData);
        try
        {
            // Ghi chuỗi JSON vào tệp seed của người host
            File.WriteAllText(hostSeedFilePath, json);
            Debug.Log("Host seed saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving host seed: " + e.Message);
        }
    }

    public static void SaveMapData(MapData mapData)
    {
        EnsureDirectoryExists();
        string json = JsonUtility.ToJson(mapData);
        try
        {
            // Ghi chuỗi JSON vào tệp
            File.WriteAllText(filePath, json);
            Debug.Log("Map data saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving map data: " + e.Message);
        }
    }

    public static MapData LoadMapData()
    {
        if (File.Exists(filePath))
        {
            try
            {
                // Đọc nội dung của tệp và chuyển đổi thành dữ liệu map
                string json = File.ReadAllText(filePath);
                MapData mapData = JsonUtility.FromJson<MapData>(json);
                return mapData;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading map data: " + e.Message);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Map data file not found!");
            return null;
        }
    }

    // Load the host seed
    public static int LoadHostSeed()
    {
        if (File.Exists(hostSeedFilePath))
        {
            try
            {
                // Đọc nội dung của tệp và chuyển đổi thành dữ liệu seed của host
                string json = File.ReadAllText(hostSeedFilePath);
                SeedData seedData = JsonUtility.FromJson<SeedData>(json);
                return seedData.seed;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading host seed: " + e.Message);
                return 0; // Return a default value if error occurs
            }
        }
        else
        {
            Debug.LogWarning("Host seed file not found!");
            return 0; // Return a default value if the file doesn't exist
        }
    }

    // Load the general seed (for the client to use)
    public static int LoadSeed()
    {
        if (File.Exists(filePath))
        {
            try
            {
                // Đọc nội dung của tệp và chuyển đổi thành dữ liệu seed
                string json = File.ReadAllText(filePath);
                SeedData seedData = JsonUtility.FromJson<SeedData>(json);
                return seedData.seed;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading seed: " + e.Message);
                return 0; // Return a default value if error occurs
            }
        }
        else
        {
            Debug.LogWarning("Seed file not found!");
            return 0; // Return a default value if the file doesn't exist
        }
    }
}

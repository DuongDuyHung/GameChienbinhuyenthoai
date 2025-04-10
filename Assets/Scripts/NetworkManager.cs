using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public bool IsHost { get; private set; }
    public TMP_Text playerQuantity;
    public MapGenerator[] mapGenerator;
    public int seed;
    public TMP_Text roomcode;
    public TMP_InputField roomcodeinput;
    private NetworkRunner networkRunner;
    [SerializeField] private NetworkPrefabRef networkPrefabRef;
    private Dictionary<PlayerRef, NetworkObject> spawnanCharacter = new Dictionary<PlayerRef, NetworkObject>();
    public async void GameStart(GameMode mode)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        mapGenerator = FindObjectsOfType<MapGenerator>();
        networkRunner.ProvideInput = true;

        IsHost = mode == GameMode.Host;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        string roomCode = string.Empty;

        if (IsHost)
        {
            if (!File.Exists(MapDataSaver.hostSeedFilePath))
            {
                seed = UnityEngine.Random.Range(0, int.MaxValue);
                MapDataSaver.SaveHostSeed(seed);
            }
            else
            {
                seed = MapDataSaver.LoadHostSeed();
            }
            roomCode = seed.ToString(); // Tạo room code ngẫu nhiên
            roomcode.text = "Mã phòng:" + roomCode; // Hiển thị mã phòng cho host
            // With this loop
            foreach (var generator in mapGenerator)
            {
                generator.GenerateMapWithSeed(seed);
            }
        }
        else
        {
            roomCode = roomcodeinput.text.Trim(); // Lấy mã phòng từ người chơi nhập
            if (string.IsNullOrEmpty(roomCode))
            {
                Debug.LogError("Room code is required to join a game.");
                return;
            }
            // Chuyển room code thành seed (chú ý rằng chúng ta cần phải đảm bảo rằng roomCode có thể chuyển đổi sang int)
            if (!int.TryParse(roomCode, out seed))
            {
                Debug.LogError("Invalid room code. It must be a valid number.");
                return;
            }
            foreach (var generator in mapGenerator)
            {
                generator.GenerateMapWithSeed(int.Parse(roomCode));
            }
        }

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomCode,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            Debug.Log(IsHost ? $"Hosting started with room code: {roomCode}" : "Joined the room successfully!");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }
    }
    private void UpdatePlayerQuantity()
    {
        playerQuantity.text = $"Số người tham gia: {spawnanCharacter.Count} / 4";
    }
    private string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Random random = new System.Random();
        return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
    }


    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server: {reason}");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            //Vector3 playerPos = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            Vector3 playerPos = new Vector3(35 + (player.RawEncoded * 3), -217, 0);
            NetworkObject networkObject = runner.Spawn(networkPrefabRef, playerPos, Quaternion.identity, player);
            spawnanCharacter.Add(player, networkObject);
            // Tìm CinemachineVirtualCamera trong prefab của nhân vật
            Cinemachine.CinemachineVirtualCamera virtualCamera = networkObject.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                // Gán camera chỉ cho LocalPlayer
                if (runner.LocalPlayer == player)
                {
                    virtualCamera.Priority = 10; // Kích hoạt camera này
                    virtualCamera.Follow = networkObject.transform;
                    virtualCamera.LookAt = networkObject.transform;
                }
                else
                {
                    virtualCamera.Priority = 0; // Không kích hoạt camera này
                }
            }
            else
            {
                Debug.LogError("CinemachineVirtualCamera not found on the spawned NetworkObject.");
            }
            // Kích hoạt PlayerController
            var playerController = networkObject.GetComponent<PlayerController>();
            var shopController = networkObject.GetComponent<ShopController>();
            var damageAble = networkObject.GetComponent<DamageAble>();
            // Tìm và gán các đối tượng UI vào PlayerController
            if (playerController != null)
            {
                // Gán Crafting Panel
                /*GameObject craftingPanelObject = GameObject.FindGameObjectWithTag("CraftingPanel");
                if (craftingPanelObject != null)
                {
                    playerController.craftingPanel = craftingPanelObject;
                }
                else
                {
                    Debug.LogError("No GameObject found with tag 'CraftingPanel'.");
                }*/

                // Gán Quest Dialog
                GameObject questDialogObject = GameObject.FindGameObjectWithTag("QuestDialog");
                if (questDialogObject != null)
                {
                    playerController.questDialog = questDialogObject;
                }
                else
                {
                    Debug.LogError("No GameObject found with tag 'QuestDialog'.");
                }

                // Gán UIMap
                GameObject uiMapObject = GameObject.FindGameObjectWithTag("UIMap");
                if (uiMapObject != null)
                {
                    UIMap uiMap = uiMapObject.GetComponent<UIMap>();
                    if (uiMap != null)
                    {
                        playerController.uiMap = uiMap;
                    }
                    else
                    {
                        Debug.LogError("UIMap script not found on GameObject with tag 'UIMap'.");
                    }
                }
                else
                {
                    Debug.LogError("No GameObject found with tag 'UIMap'.");
                }
            }
            if (shopController != null)
            {
                // Tìm và gán UIShop
                GameObject uiShopObject = GameObject.FindGameObjectWithTag("UIShop");
                if (uiShopObject != null)
                {
                    UIShop uiShop = uiShopObject.GetComponent<UIShop>();
                    if (uiShop != null)
                    {
                        shopController.uiShop = uiShop;
                    }
                    else
                    {
                        Debug.LogError("UIShop script not found on GameObject with tag 'UIShop'.");
                    }
                }
                else
                {
                    Debug.LogError("No GameObject found with tag 'UIShop'.");
                }
            }
            if (damageAble != null)
            {
                GameObject diedMenu = GameObject.FindGameObjectWithTag("DiedMenu");
                if (diedMenu != null)
                {
                    damageAble.diedMenu = diedMenu;
                }
                else
                {
                    Debug.LogError("DiedMenu object not found on GameObject with tag 'DiedMenu'.");
                }
            }
            else
            {
                Debug.LogError("PlayerController or ShopController script not found on the spawned player.");
            }
            UpdatePlayerQuantity();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnanCharacter.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnanCharacter.Remove(player);
        }
        // Kiểm tra nếu host thoát
        if (IsHost && player == runner.LocalPlayer)
        {
            Debug.Log("Host has left the game. Returning clients to HomePage.");
            foreach (var otherPlayer in spawnanCharacter.Keys.ToList())
            {
                if (otherPlayer != runner.LocalPlayer)
                {
                    runner.Shutdown(); // Dừng NetworkRunner
                    SceneManager.LoadScene("HomePage"); // Chuyển client sang Scene HomePage
                }
            }
        }
        UpdatePlayerQuantity();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (var session in sessionList)
        {
            Debug.Log($"Room found: {session.Name}");
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
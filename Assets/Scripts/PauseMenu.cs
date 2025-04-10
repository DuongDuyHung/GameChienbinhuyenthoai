using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    // Start is called before the first frame update
    public MapGenerator mapGenerator;
    public PlayerDataSaver playerData;
    public DamageAble DamageAble;
    private void Start()
    {
        StartCoroutine(AutoDisablePauseMenu());
    }
    private void Update()
    {
        playerData = GameObject.FindGameObjectWithTag("Playerr").GetComponent<PlayerDataSaver>();
        DamageAble = GameObject.FindGameObjectWithTag("Playerr").GetComponent<DamageAble>();
    }
    private IEnumerator AutoDisablePauseMenu()
    {
        while (true)
        {
            // Kiểm tra xem Player(Clone) có trong scene không
            GameObject player = GameObject.FindGameObjectWithTag("Playerr");
            if (player != null && pauseMenu.activeSelf)
            {
                // Tự động tắt pauseMenu sau 1 giây
                yield return new WaitForSeconds(1f);
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }

            yield return null; // Đợi frame tiếp theo để tiếp tục kiểm tra
        }
    }
    public void Stop()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ChoiTiep()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void LuuLai()
    {
        // Lưu dữ liệu vị trí của người chơi
        playerData.SavePlayerData();
        // Lưu vị trí của npcPrefab
        /*GameObject npc = GameObject.FindGameObjectWithTag("NPC"); // Đặt tag "NPC" cho npcPrefab trong scene
        if (npc != null)
        {
            MapDataSaver.SaveNPCPosition(npc.transform.position);
        }
        else
        {
            Debug.LogWarning("NPC not found in the scene!");
        }*/
        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        MapData mapData = mapGenerator.SaveMapData();
        MapDataSaver.SaveMapData(mapData);
    }
    // Update is called once per frame
    public void ChoiLai()
    {
        DamageAble.IsAlive = true;
        playerData.LoadPlayerData();
        // Tải lại vị trí của npcPrefab
        /*Vector3 npcPosition = MapDataSaver.LoadNPCPosition();

        // Nếu có vị trí được tải thành công, di chuyển npcPrefab đến vị trí đó
        if (npcPosition != Vector3.zero)
        {
            GameObject npc = GameObject.FindGameObjectWithTag("NPC"); // Đặt tag "NPC" cho npcPrefab trong scene
            if (npc != null)
            {
                npc.transform.position = npcPosition;
            }
            else
            {
                Debug.LogWarning("NPC not found in the scene!");
            }
        }
        else
        {
            Debug.LogWarning("NPC position not loaded!");
        }*/
        MapData mapData = MapDataSaver.LoadMapData();
        if (mapData != null)
        {
            MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
            mapGenerator.SetSeed(mapData.seed);
            mapGenerator.tileset = mapData.tileset;
            mapGenerator.specialTile = mapData.specialTile;
            mapGenerator.objectPrefabs = mapData.objectPrefabs;
            mapGenerator.GenerateMapWithSeed(mapData.seed);
        }
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void Thoat()
    {
        SceneManager.LoadScene("HomePage");
        Time.timeScale = 1;
    }
}

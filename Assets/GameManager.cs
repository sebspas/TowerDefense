using UnityEngine;

public class GameManager : MonoBehaviour
{    
    public GameObject playerPrefab;

    public GameObject PlayerInstance { get; set; }

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        var terrainGenerator = GameObject.FindObjectOfType<TerrainGenerator>();
        var player = GameObject.FindGameObjectWithTag("Player");

        // Generate Terrain
        terrainGenerator.GenerateTerrain();

        // Create Player
        PlayerInstance = Instantiate(playerPrefab, Map.Instance.GetMapCenter(), Quaternion.identity);
    }
}

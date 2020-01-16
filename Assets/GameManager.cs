using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{    
    public GameObject playerPrefab;

    GameObject playerInstance;

    public GameObject PlayerInstance { get => playerInstance; set => playerInstance = value; }

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        TerrainGenerator terrainGenerator = GameObject.FindObjectOfType<TerrainGenerator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Generate Terrain
        terrainGenerator.GenerateTerrain();

        // Create Player
        PlayerInstance = Instantiate(playerPrefab, terrainGenerator.GetMapCenter(), Quaternion.identity);
    }
}

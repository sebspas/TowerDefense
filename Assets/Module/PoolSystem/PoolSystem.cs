using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : MonoBehaviour
{
    public List<PoolItem> poolItems;

    private List<GameObject> _poolOfGameObjects;

    private static PoolSystem _instance;

    public static PoolSystem Instance { get { return _instance; } }

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

    private void Start()
    {
        _poolOfGameObjects = new List<GameObject>();
        foreach (var item in poolItems)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                _poolOfGameObjects.Add(obj);
            }
        }
    }

    public GameObject RequestGameObject(string tag)
    {
        GameObject gameObjectReturned = null;
        foreach (var gameObject in _poolOfGameObjects)
        {
            // if it's the same tag and the game object is active (not used)
            if (gameObject.CompareTag(tag) && !gameObject.activeSelf)
            {
                gameObjectReturned = gameObject;
            }
        }

        if (!gameObjectReturned)
        {
            bool foundGO = false;
            foreach (var item in poolItems)
            {
                if (item.objectToPool.tag == tag)
                {
                    if (item.shouldExpand)
                    {
                        // Create a new one
                        gameObjectReturned = Instantiate(item.objectToPool);                        
                    }
                    else
                    {
                        Debug.LogError("You requested too many GO with tag " + tag);
                    }
                }
            }

            if (!gameObjectReturned)
            {
                Debug.LogError("No game object with this tag " + tag + " are setup in this pool.");
            }
        }
        else
        {
            // Re-able the game object
            gameObjectReturned.SetActive(true);

            // Remove it from the pool
            _poolOfGameObjects.Remove(gameObjectReturned);
        }

        return gameObjectReturned;
    }

    public void AddBackToPool(GameObject gameObject)
    {
        // Disable tge game object
        gameObject.SetActive(false);

        // Move the game object to the pool location
        gameObject.transform.position = transform.position;
        gameObject.transform.rotation = transform.rotation;
    }
}

using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public List<GameObject> spawnable;
    public float delay = 5.0f;
    public float elapsedTime = 0.0f;

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnable.Count <= 0) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= delay)
        {
            Instantiate(spawnable[Random.Range(0, spawnable.Count)], transform.position, Quaternion.identity);
            elapsedTime = 0;
        }
    }
}

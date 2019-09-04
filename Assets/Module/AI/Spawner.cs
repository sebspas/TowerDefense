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

    // Update is called once per frame
    void Update()
    {
        if (spawnable.Count <= 0) return;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= delay)
        {
            GameObject goTospawn = spawnable[Random.Range(0, spawnable.Count)];

            GameObject newGO = PoolSystem.Instance.RequestGameObject(goTospawn.tag);

            // Reset position
            newGO.transform.position = transform.position;
            newGO.transform.rotation = Quaternion.identity;

            // Reset hp
            newGO.GetComponent<HealthComponent>().Reset();

            // Reset agent state
            newGO.GetComponent<Agent>().Reset();

            elapsedTime = 0;
        }
    }
}

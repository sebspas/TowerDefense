using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    public GameObject clickMarker;
    public float stopOffset = 1.0f;

    private NavMeshAgent playerNavAgent;
    private Vector3 LastPosition;
    private GameObject MarkerObject;

    void Start()
    {
        playerNavAgent = GetComponent<NavMeshAgent>();

        MarkerObject = Instantiate(clickMarker, new Vector3(0, -10, 0), Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100,
                1 << LayerMask.NameToLayer("Terrain")))
            {
                playerNavAgent.destination = hit.point;
                playerNavAgent.isStopped = false;

                if (hit.point != LastPosition)
                {
                    LastPosition = hit.point;
                    MarkerObject.transform.position = LastPosition;
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, playerNavAgent.destination) <= stopOffset)
            {
                playerNavAgent.velocity = Vector3.zero;
                playerNavAgent.isStopped = true;
            }
        }

    }
}

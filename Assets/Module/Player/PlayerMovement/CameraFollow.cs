using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float speed = 10.0f;

    public Vector2 offset = new Vector2(5, 3);

    private Transform mainCamera;
    private Transform player;

    private void Start()
    {
        mainCamera = GetComponent<Transform>();

        TryAttachToPlayer();
    }

    private void TryAttachToPlayer()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");

        if (playerGO)
        {
            player = playerGO.GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!mainCamera) return;

        if (!player)
        {
            TryAttachToPlayer();
        }

        var lerpValue = Time.deltaTime * speed;
        mainCamera.position = Vector3.Lerp(mainCamera.position, 
            new Vector3(player.position.x - offset.x, mainCamera.position.y, player.position.z - offset.y),
            lerpValue);
    }
}

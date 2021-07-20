using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    private GameMod gameMod;
    private GameObject player;
    private Transform playerTransform;
    private float rotationSpeed;

    void Awake()
    {
        gameMod = GameObject.Find("GameMod").GetComponent<GameMod>();

        player = this.gameObject;
        rotationSpeed = gameMod.GetSpeed("rotation");
        playerTransform = player.transform;
    }

    void FixedUpdate()
    {
        if (gameMod.isStart)
        {
            if (Input.GetKey(KeyCode.A))
            {
                playerTransform.rotation *= Quaternion.Euler(0f, -rotationSpeed * Time.deltaTime, 0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                playerTransform.rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
            }
        }
    }
}

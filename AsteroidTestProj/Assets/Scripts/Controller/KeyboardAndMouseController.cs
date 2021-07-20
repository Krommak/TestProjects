using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAndMouseController : MonoBehaviour
{
    private GameMod gameMod;
    private GameObject player;
    private float rotationSpeed;

    private Transform playerTransfrom;
    void Awake()
    {
        gameMod = GameObject.Find("GameMod").GetComponent<GameMod>();

        player = this.gameObject;
        rotationSpeed = gameMod.GetSpeed("rotation");
        playerTransfrom = player.transform;
    }

    void FixedUpdate()
    {
        if (gameMod.isStart)
        {
            Plane playerPlane = new Plane(Vector3.up, playerTransfrom.position);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float hitdist = 0.0f;
            if (playerPlane.Raycast(ray, out hitdist))
            {
                Vector3 targetPoint = ray.GetPoint(hitdist);

                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

                playerTransfrom.rotation = Quaternion.Slerp(playerTransfrom.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}

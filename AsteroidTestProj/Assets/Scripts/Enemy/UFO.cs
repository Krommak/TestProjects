using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    [Header("Минимальная задержка выстрела")]
    [SerializeField] private float minShotDelay;
    [Header("Максимальная задержка выстрела")]
    [SerializeField] private float maxShotDelay;
    [Header("Материал пуль НЛО")]
    [SerializeField] Material bulletMaterial;
    [Header("Transform оружия НЛО")]
    [SerializeField] Transform UFOWeapon;
    [Header("Скорость поворота в сторону игрока")]
    [SerializeField] private float step = 20f;
    float MoveDirection = 1;
    GameObject player;
    float speed;
    float screenWidth;
    GameMod gameMod;

    private Transform playerTransform;

    void Awake()
    {
        gameMod = GameObject.Find("GameMod").GetComponent<GameMod>();

        screenWidth = gameMod.GetLeftDownPoint().x * -2f;

        MoveDirection = this.gameObject.transform.position.x < 0 ? MoveDirection : -MoveDirection;

        player = GameObject.Find("Player");
        playerTransform = player.transform;
    }

    void Start()
    {
        StartCoroutine(ShotDelay());
    }

    void FixedUpdate()
    {
        if (gameMod.isStart)
        {
            speed = screenWidth / (10 / Time.deltaTime);
            this.gameObject.transform.position = new Vector3(transform.position.x + speed * MoveDirection, transform.position.y, transform.position.z);
            if (player)
            {
                Vector3 newDir = Vector3.RotateTowards(transform.forward, playerTransform.position - transform.position, step, 0.0F);
                transform.rotation = Quaternion.LookRotation(newDir);
                gameMod.Teleport(this.gameObject);
            }
        }
    }

    private void Shot()//Стрельба
    {
        gameMod.Shot(UFOWeapon.position, bulletMaterial, transform.rotation, false);

        StartCoroutine(ShotDelay());
    }

    private IEnumerator ShotDelay()//Задержка стрельбы
    {
        float delay = Random.Range(minShotDelay, maxShotDelay);
        yield return new WaitForSecondsRealtime(delay);
        Shot();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(collision.gameObject);
            gameMod.Explosion(collision.transform.position);
            gameMod.Defeat();
            Destroy(this.gameObject);
        }
    }
}

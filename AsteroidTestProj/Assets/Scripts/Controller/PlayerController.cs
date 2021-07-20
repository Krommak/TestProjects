using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("GameObject игрока")]
    public GameObject player;
    [Header("GameObject дула оружия игрока")]
    public GameObject weapon;
    [Header("Количество выстрелов в секунду")]
    [SerializeField] private int shotPerSecond = 3;
    [Header("Материал пуль игрока")]
    [SerializeField] Material bulletMaterial;
    [Header("Массив MeshRenderer объектов корпуса")]
    [SerializeField] MeshRenderer[] elementsMeshes;
    [Header("Коллайдер игрока")]
    [SerializeField] CapsuleCollider playerCollider;
    [Header("Время неуязвимости игрока при старте")]
    [SerializeField] private int secondsOfInvincibility = 3;
    private Rigidbody playerRB;
    private Vector3 saveVelocity;
    private GameMod gameMod;
    private bool cooldown = true;
    private float movementSpeed;
    private float playerMaxSpeed;
    private Transform playerTransform, weaponTransform;

    public enum controlType
    {
        keypad = 1, keypadAndMouse = 2
    }

    public controlType playerController;

    void Awake()
    {
        gameMod = GameObject.Find("GameMod").GetComponent<GameMod>();

        playerRB = player.GetComponent<Rigidbody>();
        movementSpeed = gameMod.GetSpeed("movement");
        playerMaxSpeed = gameMod.GetSpeed("max");
        playerTransform = player.transform;
        weaponTransform = weapon.transform;

        int newType = PlayerPrefs.GetInt("ControlType") == 0 ? playerController.GetHashCode() : PlayerPrefs.GetInt("ControlType");
        if (newType == 1)
        {
            player.GetComponent<KeyboardController>().enabled = true;
            player.GetComponent<KeyboardAndMouseController>().enabled = false;
        }
        else
        {
            player.GetComponent<KeyboardController>().enabled = false;
            player.GetComponent<KeyboardAndMouseController>().enabled = true;
        }
    }

    void FixedUpdate()
    {
        if (gameMod.isStart)
        {
            if (Input.GetKey(KeyCode.W) && playerRB.velocity.z <= playerMaxSpeed)
            {
                playerRB.AddForce(playerTransform.forward * movementSpeed, ForceMode.Force);
                gameMod.ActivateSoundThrust(true);
            }
            if (playerRB.velocity.z > playerMaxSpeed)
            {
                playerRB.velocity = new Vector3(0, 0, playerMaxSpeed - 0.1f);
            }
        }
    }

    void Update()
    {
        if (gameMod.isStart)
        {
            if (Input.GetKeyDown(KeyCode.Space) && cooldown || Input.GetKeyDown(KeyCode.Mouse0) && cooldown && playerController.GetHashCode() == 1)
            {
                gameMod.Shot(weaponTransform.position, bulletMaterial, playerTransform.rotation, true);
                cooldown = false;
                StartCoroutine(SetCooldown());
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                gameMod.ActivateSoundThrust(false);
            }

            gameMod.Teleport(player);
        }
    }

    private IEnumerator SetCooldown()
    {
        yield return new WaitForSecondsRealtime(1f / shotPerSecond);

        cooldown = true;
    }

    public IEnumerator PlayerStart() // Неуязвимость на первые 3 секунды
    {
        for (int i = 0; i < secondsOfInvincibility*2; i++)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            foreach (MeshRenderer element in elementsMeshes)
            {
                if (!player) break;
                element.enabled = false;
            }
            yield return new WaitForSecondsRealtime(0.25f);
            foreach (MeshRenderer element in elementsMeshes)
            {
                if (!player) break;
                element.enabled = true;
            }
        }
        if (player)
        {
        playerCollider.enabled = true;
        }
    }

    public void PausePlayer(bool isPause)
    {
        if (isPause)
        {
            Rigidbody objRB = this.gameObject.GetComponent<Rigidbody>();
            saveVelocity = objRB.velocity;
            objRB.velocity = Vector3.zero;
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody>().velocity = saveVelocity;
        }
    }

    public void ChangeControlType() 
    {
        int type = PlayerPrefs.GetInt("ControlType") == 0 ? playerController.GetHashCode() : PlayerPrefs.GetInt("ControlType");
        if (type == 1)
        {
            PlayerPrefs.SetInt("ControlType", 2);
            player.GetComponent<KeyboardController>().enabled = false;
            player.GetComponent<KeyboardAndMouseController>().enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("ControlType", 1);
            player.GetComponent<KeyboardController>().enabled = true;
            player.GetComponent<KeyboardAndMouseController>().enabled = false;
        }
    }
}

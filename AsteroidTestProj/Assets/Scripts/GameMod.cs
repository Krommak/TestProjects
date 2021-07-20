using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMod : MonoBehaviour
{
    [Header("GameObject камера")]
    [SerializeField] private Camera cam;
    [Header("Минимальное время появления НЛО")]
    [SerializeField] private float minUFODelay;
    [Header("Максимальное время появления НЛО")]
    [SerializeField] private float maxUFODelay;
    [Header("Количество астероидов на старте")]
    [SerializeField] private int asteroidsOnStart;
    [Header("Минимальная скорость астероида")]
    [SerializeField] private float asteroidSpeedMin = 2;
    [Header("Максимальная скорость астероида")]
    [SerializeField] private float asteroidSpeedMax = 5;
    [Header("Максимальная скорость астероида")]
    [SerializeField] private Material asteroidMaterial;
    [Header("Максимальная скорость пули")]
    [SerializeField] private float bulletSpeed;
    [Header("GameObject игрока")]
    [SerializeField] private GameObject player;
    [Header("RigidBody игрока")]
    [SerializeField] private Rigidbody playerRB;
    [Header("GameObject звука ускорения")]
    [SerializeField] private GameObject soundThrust;
    [Header("Префаб НЛО")]
    [SerializeField] private GameObject UFO;
    [Header("Настройки размеров астероидов")]
    [SerializeField] private Vector3 bigAsteroidScale, mediumAsteroidScale, smallAsteroidScale;
    [Header("Время задержки генерации астероидов")]
    [SerializeField] private float asteroidsRespawnDelay = 2.0f;
    [Header("Настройки скорости игрока")]
    [SerializeField] private float playerMovementSpeed = 5;
    [SerializeField] private float playerMaxSpeed = 10;
    [SerializeField] private float playerRotationSpeed = 5;
    [Header("Префаб взрыва")]
    [SerializeField] private GameObject explosionPrefab;
    [Header("Дефолтный материал объекта пула")]
    [SerializeField] private Material defaultMaterial;
    [Header("Базовый скрипт сферы")]
    public BaseSphere baseSphere;
    private Vector3 leftDownPoint;
    [HideInInspector]
    public bool isStart = false;
    protected Vector3 objRelation;
    protected List<GameObject> activeObjects = new List<GameObject>();
    protected ObjectPooling<ObjForPool> pool;
    protected UI uI;

    void Awake()
    {
        uI = GameObject.Find("Canvas").GetComponent<UI>();
        pool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>().GetPool();

        leftDownPoint = cam.ScreenToWorldPoint(Vector3.zero);
    }

    #region Ограничение перемещения объектов и выстрел
    public void Teleport(GameObject obj) // ограничение перемещения областью экрана (принимает проверяемый объект, если второй аргумент true то объект будет уничтожен)
    {
        objRelation = cam.WorldToViewportPoint(obj.transform.position);
        float objTransformPosX = obj.transform.position.x;
        float objTransformPosZ = obj.transform.position.z;
        if (objRelation.x > 1.0f || objRelation.x < 0.0f)
        {
            if (obj.tag == "Bullet" || obj.tag == "UFO")
            {
                if (obj.tag == "UFO")
                {
                    Destroy(obj);
                    return;
                }
                else
                {
                    obj.GetComponent<Bullet>().Deactivate(obj);
                }
            }
            obj.transform.position = new Vector3(-objTransformPosX < 0 ? -objTransformPosX + 0.25f : -objTransformPosX - 0.25f, 0, obj.transform.position.z);
        }
        if (objRelation.y > 1.0f || objRelation.y < 0.0f)
        {
            if (obj.tag == "Bullet" || obj.tag == "UFO")
            {
                if (obj.tag == "UFO")
                {
                    Destroy(obj);
                    return;
                }
                else
                {
                    obj.GetComponent<Bullet>().Deactivate(obj);
                }
            }
            obj.transform.position = new Vector3(obj.transform.position.x, 0, -objTransformPosZ < 0 ? -objTransformPosZ + 0.25f : -objTransformPosZ - 0.25f);
        }
    }

    public void Shot(Vector3 pos, Material color, Quaternion direction, bool playerIsOwner)//Выстрел. Принимает позицию генерации пули, цвет, направление и родителя.
    {
        GameObject res = pool.GetFreeElement().gameObject;
        res.transform.position = pos;
        res.transform.rotation = direction;
        baseSphere.ActivateBullet(res, bulletSpeed, color, playerIsOwner);
    }
    #endregion

    #region Астероиды
    public void SpawnAsteroids(string type) // Генерация количства астероидов(count) размера (type), type - "big", "medium", "small" 
    {
        int count = asteroidsOnStart;
        Vector3 randomPos;
        Vector3 randomAngles;

        for (int i = 0; i < count; i++)
        {
            GameObject res = pool.GetFreeElement().gameObject;
            randomPos = GetNewPosition();
            randomAngles = new Vector3(Random.Range(0f, 90f), 0, 0);
            res.transform.position = randomPos;
            res.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 90), 0f);
            res.transform.localScale = bigAsteroidScale;
            baseSphere.ActivateAsteroid(res, Random.Range(asteroidSpeedMin, asteroidSpeedMax), "big");
        }
    }

    private Vector3 GetNewPosition() //Возвращает позицию для появления астероидов
    {
        float leftDownX = leftDownPoint.x;
        float leftDownZ = leftDownPoint.z;
        Vector3 randomPos = Vector3.zero;
        bool isLeft = Random.Range(0, 2) == 1 ? true : false;
        bool isTop = Random.Range(0, 2) == 1 ? true : false;
        bool isTopBottom = Random.Range(0, 2) == 1 ? true : false;
        float randomPosX;
        float randomPosZ;

        if (isTopBottom)
        {
            randomPosX = Random.Range(leftDownX, -leftDownX);
            randomPosZ = isTop ? Random.Range(-leftDownZ, -leftDownZ * 0.99f) : Random.Range(leftDownZ, leftDownZ * 0.99f);
        }
        else
        {
            randomPosX = isLeft ? Random.Range(leftDownX, leftDownX * 0.99f) : Random.Range(-leftDownX, -leftDownX * 0.99f);
            randomPosZ = Random.Range(leftDownZ, -leftDownZ);
        }

        randomPos = new Vector3(randomPosX, 0, randomPosZ);

        return randomPos;
    }

    public void RespawnAsteroids() // Проверка и запуск генерации астероидов после уничтожения всех астероидов
    {
        StartCoroutine(RespawnDelay());
    }

    private IEnumerator RespawnDelay() //задержка генерации астероидов
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (pool.GetActiveElementsNum() == 0)
        {
            yield return new WaitForSecondsRealtime(asteroidsRespawnDelay);
            asteroidsOnStart++;
            SpawnAsteroids("big");
        }
    }
    #endregion

    #region НЛО
    private Vector3 GetUFOPos() //Получение позиции для генерации НЛО
    {
        float leftDownX = leftDownPoint.x;
        float leftDownZ = leftDownPoint.z;

        Vector3 randomPos = Vector3.zero;
        bool isLeft = Random.Range(0, 2) == 1 ? true : false;
        float randomPosX;
        float randomPosZ;

        if (isLeft)
        {
            randomPosX = leftDownX * 0.95f;
            randomPosZ = Random.Range(leftDownZ * 0.8f, -leftDownZ * 0.8f);
        }
        else
        {
            randomPosX = -leftDownX * 0.95f;
            randomPosZ = Random.Range(leftDownZ * 0.8f, -leftDownZ * 0.8f);
        }

        randomPos = new Vector3(randomPosX, 0, randomPosZ);

        return randomPos;
    }


    public IEnumerator RespawnUFODelay() //Задержка появления НЛО
    {
        float delay = Random.Range(minUFODelay, maxUFODelay);
        yield return new WaitForSecondsRealtime(delay);
        SpawnUFO();
    }

    public void SpawnUFO() //Генерация НЛО
    {
        Vector3 pos = GetUFOPos();
        GameObject res = Instantiate(UFO, pos, Quaternion.Euler(0f, 90f, 0f));
    }
    #endregion

    #region Функции паузы, звука ускорения, генерация и удаление взрывов, очистка сцены
    public void GamePause(bool isPause) // Пауза
    {
        activeObjects = pool.GetActiveObjects();
        if (activeObjects.Count != 0)
        {
            for (int i = 0; i < activeObjects.Count; i++)
            {
                baseSphere.Pause(isPause, activeObjects[i]);
            }
        }

        activeObjects.Clear();

        if (player)
            player.GetComponent<PlayerController>().PausePlayer(isPause);
    }

    public void Defeat() // Поражение
    {
        GamePause(true);
        StartCoroutine(uI.EndGame());
    }

    public void Explosion(Vector3 pos) // Взрыв
    {
        GameObject res = Instantiate(explosionPrefab, pos, Quaternion.identity);
        StartCoroutine(DeleteExplosion(res));
    }

    private IEnumerator DeleteExplosion(GameObject obj)
    {
        yield return new WaitForSecondsRealtime(1f);

        Destroy(obj);
    }

    public void ActivateSoundThrust(bool isThrust) // Звук ускорения
    {
        soundThrust.SetActive(isThrust);
    }

    public void RevertGame() // Очистка сцены
    {
        activeObjects = pool.GetActiveObjects();
        if (activeObjects.Count != 0)
        {
            for (int i = 0; i < activeObjects.Count; i++)
            {
                activeObjects[i].GetComponent<Asteroid>().Deactivate(activeObjects[i]);
            }
        }
        activeObjects.Clear();

        GameObject UFO = GameObject.Find("UFO");
        if (UFO)
        {
            Destroy(UFO);
        }

        if (player)
        {
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
        }
    }
    #endregion

    #region Геттеры
    public float GetMinAsteroidSpeed(bool minSpeed)
    {
        if (minSpeed)
        {
            return asteroidSpeedMin;
        }
        else
            return asteroidSpeedMax;
    }

    public Vector3 GetAsteroidScale(string type)
    {
        Vector3 scale = Vector3.zero;
        switch (type)
        {
            case "big":
                scale = bigAsteroidScale;
                break;
            case "medium":
                scale = mediumAsteroidScale;
                break;
            case "small":
                scale = smallAsteroidScale;
                break;
        }
        return scale;
    }

    public float GetSpeed(string speedType)
    {
        float speed = 0;
        if (speedType == "movement")
        {
            speed = playerMovementSpeed;
        }
        else if (speedType == "max")
        {
            speed = playerMaxSpeed;
        }
        else if (speedType == "rotation")
        {
            speed = playerRotationSpeed;
        }
        return speed;
    }
    public Vector3 GetLeftDownPoint()
    {
        return leftDownPoint;
    }

    public Material GetDefaultMaterial()
    {
        return defaultMaterial;
    }

    #endregion

}

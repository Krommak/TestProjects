using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : BaseSphere
{
    private string asteroidType;
    private ObjectPooling<ObjForPool> pool;
    void Start()
    {
        pool = GameObject.Find("ObjectPool").GetComponent<ObjectPool>().GetPool();
    }

    void FixedUpdate()
    {
        gameMod.Teleport(this.gameObject);
    }


    new private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "UFO")
        {
            if (collision.gameObject.tag == "UFO")
            {
                gameMod.Explosion(collision.gameObject.transform.position);
                Destroy(collision.gameObject);
                gameMod.time = 0;
                gameMod.SetNewDelay();
                gameMod.UFOOnGame = false;
            }
            if (collision.gameObject.tag == "Player")
            {
                gameMod.Explosion(collision.gameObject.transform.position);
                Destroy(collision.gameObject);
                gameMod.Defeat();
            }
            Deactivate(this.gameObject);
        }
    }

    public void SetAsteroidType(string type)
    {
        this.asteroidType = type;
    }
    public string GetAsteroidType()
    {
        return this.asteroidType;
    }

    public void AsteroidSplit(GameObject parent)
    {
        Asteroid parentAsteroidComponent = parent.GetComponent<Asteroid>();
        string type = parentAsteroidComponent.GetAsteroidType();

        if (type == "small")
        {
            Deactivate(parent);
            gameMod.RespawnAsteroids();
        }
        else
        {
            GameObject res;

            float asteroidSpeed = Random.Range(gameMod.GetMinAsteroidSpeed(true), gameMod.GetMinAsteroidSpeed(false));

            for (int i = 0; i < 2; i++)
            {
                res = pool.GetFreeElement().gameObject;
                res.tag = "Asteroid";
                string newType = type == "big" ? "medium" : "small";
                Vector3 newScale = type == "big" ? gameMod.GetAsteroidScale("medium") : gameMod.GetAsteroidScale("small");
                res.tag = "Asteroid";
                res.transform.rotation = parent.transform.rotation;
                res.transform.Rotate(new Vector3(0.0f, i == 1 ? -45f : 45f, 0.0f), Space.Self);
                ActivateAsteroid(res, asteroidSpeed, newType);
                Asteroid newAsteroidComponent = res.GetComponent<Asteroid>();
                newAsteroidComponent.enabled = true;
                newAsteroidComponent.SetAsteroidType(newType);

                res.transform.localScale = newScale;
                res.transform.position = parent.transform.position;
            }

            Deactivate(parent);
        }
    }
}

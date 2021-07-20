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

    private IEnumerator ColliderEnable()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        this.gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    new private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == this.gameObject.tag)
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>(), true);
        }
        else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "UFO")
        {
            if (collision.gameObject.tag == "UFO")
            {
                gameMod.Explosion(collision.gameObject.transform.position);
                Destroy(collision.gameObject);
                gameMod.RespawnUFODelay();
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
                Asteroid newAsteroidComponent = res.AddComponent<Asteroid>();
                newAsteroidComponent.enabled = true;
                // Vector3 newPosition = new Vector3(i == 1 ? parent.transform.right.x - 0.25f : parent.transform.right.x + 0.25f,
                //                                     parent.transform.position.y,
                //                                     parent.transform.position.z);
                Vector3 newScale = type == "big" ? gameMod.GetAsteroidScale("medium") : gameMod.GetAsteroidScale("small");
                string newType = type == "big" ? "medium" : "small";
                res.tag = "Asteroid";
                res.transform.rotation = parent.transform.rotation;
                res.transform.Rotate(new Vector3(0.0f, i == 1 ? -45f : 45f, 0.0f), Space.Self);
                newAsteroidComponent.SetAsteroidType(newType);
                ActivateAsteroid(res, asteroidSpeed, newType);
                res.transform.localScale = newScale;
                // res.transform.position = newPosition;
                StartCoroutine(newAsteroidComponent.ColliderEnable());
                res.transform.position = parent.transform.position;
            }

            parent.GetComponent<Asteroid>().Deactivate(parent);
        }
    }
}

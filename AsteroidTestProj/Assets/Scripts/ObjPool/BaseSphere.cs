using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSphere : MonoBehaviour
{
    // дефолтные переменные для объектов пула
    private Vector3 standartScale = new Vector3(0.1f, 0.1f, 0.1f);

    protected GameMod gameMod;
    protected UI uI;

    void Awake()
    {
        gameMod = GameObject.Find("GameMod").GetComponent<GameMod>();
        uI = GameObject.Find("Canvas").GetComponent<UI>();
    }

    public void Move(GameObject obj, float speed)
    {
        obj.GetComponent<Rigidbody>().AddForce(obj.transform.forward * speed);
    }

    public void ActivateBullet(GameObject obj, float speed, Material color, bool playerIsOwner)
    {
        Bullet bulletComponent = obj.AddComponent<Bullet>();
        bulletComponent.enabled = true;
        bulletComponent.SetOwner(playerIsOwner);
        obj.GetComponent<MeshRenderer>().material = color;
        obj.tag = "Bullet";
        obj.SetActive(true);
        Move(obj, speed);
    }

    public void ActivateAsteroid(GameObject obj, float speed, string type)
    {
        obj.tag = "Asteroid";
        Asteroid asteroidComponent = obj.AddComponent<Asteroid>();
        asteroidComponent.enabled = true;
        obj.GetComponent<Asteroid>().SetAsteroidType(type);
        obj.transform.localScale = gameMod.GetAsteroidScale(type);
        Move(obj, speed);
    }

    public void Deactivate(GameObject obj)
    {
        ObjForPool objBaseComponent = obj.GetComponent<ObjForPool>();
        objBaseComponent.transform.localScale = standartScale;
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.GetComponent<AudioSource>().enabled = false;
        Bullet bulletComponent = obj.GetComponent<Bullet>();
        Asteroid asteroidComponent = obj.GetComponent<Asteroid>();
        if(bulletComponent)
        {
            Destroy(bulletComponent);    
        }
        else
        {
            Destroy(asteroidComponent);
        }
        obj.GetComponent<MeshRenderer>().material = gameMod.GetDefaultMaterial();
        obj.SetActive(false);
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    public void Pause(bool isPause, GameObject obj)
    {
        if (isPause)
        {
            Rigidbody objRB = obj.gameObject.GetComponent<Rigidbody>();
            obj.GetComponent<ObjForPool>().saveVelocity = objRB.velocity;
            objRB.velocity = Vector3.zero;
        }
        else
        {
            obj.gameObject.GetComponent<Rigidbody>().velocity = obj.GetComponent<ObjForPool>().saveVelocity;
        }
    }
}

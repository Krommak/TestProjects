using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseSphere
{
    private bool playerIsOwner;

    void Start()
    {
        this.gameObject.GetComponent<AudioSource>().enabled = true;
    }

    void FixedUpdate()
    {
        gameMod.Teleport(this.gameObject);
    }

    public void SetOwner(bool isPlayer)
    {
        playerIsOwner = isPlayer;
    }
    
    public bool GetOwner()
    {
        return playerIsOwner;
    }

    new private void OnCollisionEnter(Collision collision) // 
    {
        if (collision.gameObject.tag == this.gameObject.tag)
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>(), true);
        }
        else if (collision.gameObject.tag == "Asteroid")
        {
            gameMod.Explosion(this.gameObject.transform.position);
            collision.gameObject.GetComponent<Asteroid>().AsteroidSplit(collision.gameObject);
            uI.SetScore(collision.gameObject, this.gameObject);
            Deactivate(this.gameObject);
        }
        else if (collision.gameObject.tag == "UFO")
        {
            gameMod.Explosion(collision.gameObject.transform.position);
            Destroy(collision.gameObject);
            gameMod.RespawnUFODelay();
            Deactivate(this.gameObject);
            uI.SetScore(collision.gameObject, this.gameObject);
        }
        else if(collision.gameObject.tag == "Player")
        {
            gameMod.Explosion(collision.gameObject.transform.position);
            Destroy(collision.gameObject);
            gameMod.Defeat();
        }
    }
}

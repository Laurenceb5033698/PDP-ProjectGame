﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    [SerializeField] protected float damage;
    [SerializeField] protected float lifetime = 5f;//lifetime in seconds
    [SerializeField] protected string ownertag; //(eg player)
    [SerializeField] protected float speed;//forward speed
    [SerializeField] protected GameObject psImpactPrefab;//particleSystem prefab


    // Use this for initialization
    void Start () {
        //damage = 10f;
        //speed = 10f;
	    //test self collision
        ///ownertag = "PlayerShip";
	}
    public void SetupValues(float dmg, float spd,string str)
    {
        ownertag = str;
        damage = dmg;
        speed = spd;
    }
    void OnTriggerEnter(Collider other)
    {//deal damage to target
        //Debug.Log("Entity hit: " + other.gameObject.name);

        if ((other != null) && (other.gameObject.tag != ownertag) && (other.gameObject.GetComponent<Projectile>() == null))
        {//successful collision that wasnt with shooter
            //Debug.Log("other Entity: " + other.gameObject.tag);
            switch (other.gameObject.tag)
            {
                case "PlayerShip":
                    other.gameObject.GetComponentInParent<ShipController>().TakeDamage(damage);
                    applyImpulse(other.GetComponentInParent<Rigidbody>());
                    break;
                case "EnemyShip":
                    other.gameObject.GetComponentInParent<NewBasicAI>().TakeDamage(damage);
                    applyImpulse(other.GetComponentInParent<Rigidbody>());
                    break;
                case "Asteroid":
                    other.gameObject.GetComponent<Asteroid>().TakeDamage(damage);
                    applyImpulse(other.GetComponent<Rigidbody>());
                    break;
                case "shard":
                    Destroy(other.transform.gameObject);
                    break;
                default:
                    Debug.Log("Unknown entity. " + other.gameObject.tag);

                    break;
            }
            
            
            DestroySelf();
        }
    }

    protected void applyImpulse(Rigidbody body)
    {
        //Vector3 direction = transform.position - body.transform.position;
        body.AddForce(transform.forward * ((damage/2)+(speed/(2+body.mass))), ForceMode.Impulse);
    }

    // Update is called once per frame
    protected virtual void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
            DestroySelf();
	}

    protected virtual void DestroySelf()
    {// perhaps spawn a particle? like missile does
        Instantiate(psImpactPrefab, transform.position, transform.rotation);
        Destroy(transform.gameObject);
    }
}

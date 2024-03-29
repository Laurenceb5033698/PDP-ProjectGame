﻿using UnityEngine;
using System.Collections;

public class NewBasicAI : MonoBehaviour {

    private int id;
    private int type;
    private GameObject sm;
    private const int scoreValue = 100;

    //Customise AI range behaviours
    [SerializeField] private float boostRange = 30f;
    [SerializeField] private GameObject ship; // enemy ship object
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ShipStats stats;
    [SerializeField] private Arsenal arsenal;
    [SerializeField] private GameObject psDestructPrefab;


    [SerializeField] private Vector3 destination;
    private GameObject player;

    private bool spawnedPickup = false;

	// Use this for initialization
	void Awake () {
        stats = gameObject.GetComponent<ShipStats>();
        ship = gameObject;
        rb = ship.gameObject.GetComponent<Rigidbody>();

        arsenal = GetComponentInChildren<Arsenal>();
        if (arsenal == null) Debug.Log("No weapon attached.");
        else
            arsenal.SetShipObject(ship);
	}
	
	// Update is called once per frame
	void Update () {
        if (stats.IsAlive())
        {
            destination = player.transform.position;
            move();
        }
        else
        {
            if (!spawnedPickup) spawnedPickup = sm.GetComponent<PickupManager>().SpawnPickup(transform.position);
            Instantiate(psDestructPrefab, transform.position, transform.rotation);
            DestroySelf();
        }
	}
    private void move()
    {
        float dist = Vector3.Distance(destination, gameObject.transform.position);
        float currentSpeed = (dist <= boostRange) ? stats.GetBoostSpeed() : stats.GetMainThrust();//use speeds from shipStats. Change on prefabs

        Vector3 controlDir = (destination - ship.transform.position).normalized;
        float angle = Vector3.Angle(controlDir, gameObject.transform.forward);

        if (Vector3.Cross(controlDir, gameObject.transform.forward).y < 0) angle = -angle;
        angle = angle / -180;

        rb.AddTorque(Vector3.up * (((angle) * stats.GetRotSpeed()) ) * Time.deltaTime);
        rb.AddForce(gameObject.transform.forward * currentSpeed * 20 * Time.deltaTime, ForceMode.Acceleration);

        if (dist <= 50 && arsenal != null) arsenal.FireWeapon(controlDir);
    }

    public void TakeDamage(float amount)
    {
        stats.TakeDamage(amount);
    }

    // enemy spawner related functions
    void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.GetComponent<Projectile>()) rb.AddForce((ship.transform.position - c.gameObject.transform.position) * 18, ForceMode.Impulse);
        stats.TakeDamage(10 * Time.deltaTime);
    }
    private void DestroySelf()
    {
        sm.GetComponent<GameManager>().AddScore(scoreValue);
        if (type != -1) sm.GetComponent<EnemyManager>().RemoveShip(id, type);
        Destroy(transform.gameObject);
    }
    public void Initalise(GameObject go, GameObject s, int i, int t)
    {
        player = go;
        sm = s;
        id = i;
        type = t;
    }
    public int GetId()
    {
        return id;
    }
}

﻿using UnityEngine;
using System.Collections;

public class NewBasicAI : MonoBehaviour {

    [SerializeField] private GameObject ship;
    [SerializeField] private Rigidbody rb;
    //[SerializeField] private GameObject target;
    [SerializeField] private Vector3 destination;
    [SerializeField] private int state = 0;
    [SerializeField] private GameObject sceneManager;
    [SerializeField] private ShipStats stats;

    [SerializeField] private GameObject player;

    private LineRenderer laser;
    private GameObject laserTarget = null;

    [SerializeField] private Material idleLaserColor;

	// Use this for initialization
	void Start () {


        stats = gameObject.AddComponent<ShipStats>();
        ship = gameObject;
        rb = ship.gameObject.GetComponent<Rigidbody>();

        laser = gameObject.GetComponent<LineRenderer>();
        laser.SetWidth(0.2f, 0.2f);

        laser.GetComponent<Renderer>().material = idleLaserColor;
        laser.SetPosition(0, ship.transform.position);
        laser.SetPosition(1, ship.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        laser.SetPosition(0, ship.transform.position);

        if (stats.ShipHealth > 0)
        {
            destination = player.transform.position;
            laser.SetPosition(1, destination);

            move();
        }
        else
        {//Ship Died
            //spawn pickup
            if (Random.Range(0f, 10f) > 3f)
            {
                GameObject mpickup = sceneManager.GetComponent<PickupManager>().GetRandomPickup();
                Instantiate(mpickup, ship.transform.position, Quaternion.identity); 	// create gameobject
            }
            laser.SetPosition(1, ship.transform.position);

            Destroy(transform.gameObject);
        }
	}
    private void move()
    {
        Vector3 targetDir = destination - ship.transform.position;
        Vector3 controlDir = targetDir.normalized;

        float angle = Vector3.Angle(controlDir, gameObject.transform.right);
        Vector3 cross = Vector3.Cross(controlDir, gameObject.transform.right);
        if (cross.y < 0) angle = -angle;
        angle = angle / -180;
        Debug.Log(rb.angularVelocity.magnitude);//- rb.angularVelocity.magnitude
        rb.AddTorque(Vector3.up * (((angle) * 250f) ) * Time.deltaTime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(controlDir), (((angle) * 250f) - rb.angularVelocity.magnitude) * Time.deltaTime);
        //rb.velocity
        if (Vector3.Distance(destination, gameObject.transform.position) <= 30)
        {
            rb.AddForce(gameObject.transform.right * 3000 * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(gameObject.transform.right * 1000 * Time.deltaTime);
        }
        
    }
}

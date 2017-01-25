﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ShipController : MonoBehaviour
{
    private GameObject ship;  // ship gameobject
    private Inputs controls;
    [SerializeField] private GameObject mPreF; // missile prefab
    [SerializeField] private GameObject turret; // missile prefab
    [SerializeField] private Weapon gun;

    private Rigidbody rb; 	// ship's rigid body
    private ShipStats stats;
    private Shield shield;

    // Mains --------------------------------------------------------------------------------------------------------
    void Start() // Use this for initialization
    {
        ship = transform.gameObject;
        controls = ship.GetComponent<Inputs>();
        rb = ship.GetComponent<Rigidbody>();
        stats = ship.GetComponent<ShipStats>();
        shield = ship.GetComponentInChildren<Shield>();
        gun = turret.GetComponent<Weapon>();
    }

    void Update() // Update is called once per frame
    {
        stats.LaserState = controls.RLaser;

        if (controls.rocket && stats.LoadMissile())
        {
            Instantiate(mPreF, ship.transform.position + ship.transform.right * 6f, Quaternion.LookRotation(ship.transform.right, Vector3.up));
            stats.DecreaseMissileAmount();
        }
        if (Mathf.Abs(controls.yawAxis) > 0.1f || Mathf.Abs(controls.rightY) > 0.1f)//shooting
        {
            gun.Shoot(new Vector3(controls.yawAxis, 0, controls.rightY).normalized);
        }
        if (stats.IsAlive())
        {
            MoveShip();
        }
    }

    // FUNCTIONS --------------------------------------------------------------------------------------------------------	

    private void MoveShip()
    {
        float boostMultiplier = (controls.boost) ? 2 : 1;

        //rb.velocity = transform.TransformDirection(new Vector3(controls.zAxis * stats.GetMainThrust(), 0, -controls.xAxis * stats.GetMainThrust())) * boostMultiplier;
        rb.velocity = new Vector3(controls.xAxis * stats.GetMainThrust(), 0, controls.zAxis * stats.GetMainThrust()) * boostMultiplier;
        rb.angularVelocity = new Vector3(0,0,0);

        //if (Mathf.Abs(controls.yawAxis) > 0.1f || Mathf.Abs(controls.rightY) > 0.1f)
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(-controls.rightY, 0, controls.yawAxis)), 10);
        if (Mathf.Abs(controls.xAxis) > 0.1f || Mathf.Abs(controls.zAxis) > 0.1f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(controls.xAxis, 0, controls.zAxis)) * Quaternion.Euler(new Vector3(0, -90, 0)), 10);
    }

    // EVENT HANDLERS-------------------------------------------------------------------------------------
    void OnCollisionEnter(Collision c)
    {
        TakeDamage(c.relativeVelocity.magnitude / 2);
    }

    public void TakeDamage(float amount)
    {
        stats.TakeDamage(amount);
    }
}

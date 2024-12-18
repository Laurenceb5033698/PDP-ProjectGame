﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using XInputDotNetPure; // for controller rumble

public class ShipController : MonoBehaviour
{
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;



    private GameObject ship;  // ship gameobject
    private Inputs controls;
    [SerializeField] private GameObject mPreF; // missile prefab
    [SerializeField] private Arsenal arsenal;
    private int weaponType =0;
    private bool usedEquipment = false;
    private bool aiming = false;
    private float rumbleTimer = 0;

    private Rigidbody rb; 	// ship's rigid body
    private ShipStats stats;
    public ParticleSystem shield_Emitter;

    // Mains --------------------------------------------------------------------------------------------------------
    void Awake() // Use this for initialization
    {


        ship = transform.gameObject;
        controls = ship.GetComponent<Inputs>();
        rb = ship.GetComponent<Rigidbody>();
        stats = ship.GetComponent<ShipStats>();
        //shield = ship.GetComponentInChildren<Shield>();
        arsenal = ship.GetComponentInChildren<Arsenal>();
        arsenal.SetShipObject(ship);
    }
    void Start()
    {
        arsenal.RegisterUI();

    }
    private void OnEnable()
    {
        //UIManager.GamevolumeChanged += ShootVolumeChanged;

    }
    private void OnDisable()
    {
        //UIManager.GamevolumeChanged -= ShootVolumeChanged;

    }
    void Update() // Update is called once per frame
    {
        UpdateController();

        aiming = (Mathf.Abs(controls.RightStick.x) > 0.1f || Mathf.Abs(controls.RightStick.y) > 0.1f);

        if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetMouseButtonDown(1)) // left bumper
        {
            weaponType = arsenal.ChangeGun(-1);
            
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            weaponType = arsenal.ChangeGun(1);
        }


        Vector3 direction = Vector3.zero;

        if (aiming)
        {
            direction = new Vector3(controls.RightStick.x, 0, controls.RightStick.y).normalized;
            arsenal.FireWeapon(direction);
        }
        else if (Input.GetMouseButton(0))
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitdist = 0.0f;
            if (playerPlane.Raycast(ray, out hitdist))
            {
                Vector3 targetPoint = ray.GetPoint(hitdist);
                direction = (targetPoint - transform.position).normalized;


                arsenal.FireWeapon(direction);
                //Debug.Log(direction);
            }


        }
        else direction = transform.right;

        if (!usedEquipment && (Input.GetAxis("RightTrigger") > 0.1f || Input.GetKeyDown(KeyCode.Space)) && stats.LoadMissile()) // right bumper
        {
            usedEquipment = true;
            Instantiate(mPreF, ship.transform.position + direction * 8f, Quaternion.LookRotation(direction, Vector3.up));
            stats.DecreaseMissileAmount();
        }
        if (Input.GetAxis("RightTrigger") < 0.1f)
            usedEquipment = false;

        if (stats.IsAlive()) MoveShip();
        else { rumbleTimer = 0; GamePad.SetVibration(playerIndex, 0, 0); }
    }

    void FixedUpdate()
    {
        if (stats.ShipShield < 0.1f) stats.ActivateShieldPU();

        //GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);

        if (rumbleTimer > Time.time)
            GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
        else
            GamePad.SetVibration(playerIndex, 0, 0);
    }


    // FUNCTIONS --------------------------------------------------------------------------------------------------------	
    private void UpdateController()
    {
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        prevState = state;
        state = GamePad.GetState(playerIndex);
    }
    private void MoveShip()
    {
        float currentSpeed = 0.0f;
        currentSpeed = stats.GetMainThrust();


        if (Mathf.Abs(controls.LeftStick.x) > 0.1f || Mathf.Abs(controls.LeftStick.y) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(controls.LeftStick.x, 0, controls.LeftStick.y)) * Quaternion.Euler(new Vector3(0, -90, 0)), 10);

            if ((Input.GetAxis("LeftTrigger") > 0.1f || Input.GetKey(KeyCode.LeftShift)) && stats.ShipFuel > 0.1f)
            {
                currentSpeed = stats.GetBoostSpeed();
                stats.ShipFuel = -25 * Time.deltaTime;
            }
        }

        rb.linearVelocity = new Vector3(controls.LeftStick.x * currentSpeed, 0, controls.LeftStick.y * currentSpeed);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    // EVENT HANDLERS-------------------------------------------------------------------------------------
    void OnCollisionEnter(Collision c)
    {
        TakeDamage(c.transform.position, c.relativeVelocity.magnitude / 4);
        if (c.gameObject.tag == "EnemyShip")
        {
            c.gameObject.GetComponent<NewBasicAI>().TakeDamage(transform.position, 50);
        }
    }

    private void Shield_effect(Vector3 other)
    {
        //Debug.Log("Collision Entered: " + other.gameObject.name);
        Vector3 dir = other - shield_Emitter.transform.position;
        dir.Normalize();
        shield_Emitter.transform.rotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(0,-90,0);
        
        shield_Emitter.Play();
    }

    public void TakeDamage(Vector3 otherpos, float amount)
    {
        if (stats.ShipShield > 0)
            Shield_effect(otherpos);

        stats.TakeDamage(amount);
        rumbleTimer = Time.time + 0.3f;
    }
    public int GetWeaponType()
    {
        return weaponType;
    }

    void OnApplicationQuit()
    {
        GamePad.SetVibration(playerIndex, 0, 0);
    }

    void ShootVolumeChanged(bool x)
    {
        arsenal.volumeChanged(PlayerPrefs.GetFloat("gameVolume") /10);
    }
}

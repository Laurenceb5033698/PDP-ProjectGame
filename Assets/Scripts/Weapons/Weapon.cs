﻿using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour{

    [SerializeField] private GameObject bullet1;
    [SerializeField] private GameObject bullet2;


    [SerializeField] private GameObject ship;
    //private Inputs controls = null;
    [SerializeField] private float shootSpeed;//fiddle with this one

    enum WeaponFlavour { Pew, Tri };

    [SerializeField] private WeaponFlavour WeaponType = WeaponFlavour.Pew;
    private float reload = 0;

    [SerializeField] private float bulletDamage = 5f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float spread = 2;


    private AudioSource shootSound;

    void Start () // Use this for initialization
    {
        shootSound = GetComponent<AudioSource>();
    }

    public void changeType(string weap)
    {
        switch (weap)
        {
            case "tri":
                WeaponType = WeaponFlavour.Tri;
                break;
            case "pew":
            default:
                WeaponType = WeaponFlavour.Pew;
                break;
        }
    }

	void Update () // Update is called once per frame
    {
	}

    public void SetShipObject(GameObject obj)
    {
        ship = obj;
    }
    private void spawnProjectile(Vector3 aimDir)
    {//spawn pattern for weapon type
        GameObject mBullet;
        switch (WeaponType)
        {
            case WeaponFlavour.Tri:
                //three instances
                Vector3 spreadera = (aimDir * 6f) + (Vector3.Cross(aimDir, Vector3.up) * spread);//spread is an arbitrary value which increases the angle of spread
                Vector3 spreaderb = (aimDir * 6f) - (Vector3.Cross(aimDir, Vector3.up) * spread);//spread is an arbitrary value which increases the angle of spread

                mBullet = (GameObject)Instantiate(bullet2, ship.transform.position + spreadera, Quaternion.LookRotation(spreadera.normalized, Vector3.up));
                mBullet.GetComponent<Projectile>().SetupValues(bulletDamage/2, bulletSpeed, ship.tag);

                mBullet = (GameObject)Instantiate(bullet2, ship.transform.position + aimDir * 6f, Quaternion.LookRotation(aimDir, Vector3.up));
                mBullet.GetComponent<Projectile>().SetupValues(bulletDamage/2, bulletSpeed, ship.tag);

                mBullet = (GameObject)Instantiate(bullet2, ship.transform.position + spreaderb, Quaternion.LookRotation(spreaderb.normalized, Vector3.up));
                mBullet.GetComponent<Projectile>().SetupValues(bulletDamage/2, bulletSpeed, ship.tag);
                break;
            case WeaponFlavour.Pew:
            default://pew
                mBullet = (GameObject)Instantiate(bullet1, ship.transform.position + aimDir * 6f, Quaternion.LookRotation(aimDir, Vector3.up));
                mBullet.GetComponent<Projectile>().SetupValues(bulletDamage, bulletSpeed, ship.tag);
                break;

        }
    }
    public void Shoot(Vector3 direction)
    {
        if(Time.time > reload)
        {
            reload = Time.time + shootSpeed;
            spawnProjectile(direction);

            shootSound.Play();
        }

        
        //shootSound.Play(44100);


    }

}

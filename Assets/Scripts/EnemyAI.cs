﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic; 


public class EnemyAI : MonoBehaviour {

    [SerializeField] private GameObject ship;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 destination;
    [SerializeField] private int state = 0;

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private bool laserIsOn = false;

    private Vector3 origin;
    private float timer;
    // 0 = idle, 1 = searchForTarget, 2 = patrolArond, 3 = gotoDestination, 4 = attackTarget, 5 = returnHome

    //pathing variables

    private RaycastHit pathObjectHitInfo;
    private Vector3 newDest;
    [SerializeField] private List<Vector3> pathList = new List<Vector3>();
    //private List<Vector3> PatrolPathList = new List<Vector3>();
    

	// Use this for initialization
	void Start ()
    {
        ship = transform.gameObject;
        rb = ship.gameObject.GetComponent<Rigidbody>();
        destination = ship.transform.position;
        origin = ship.transform.position;
        //target = null;
        timer = Time.time;
        state = 0;
        
        


        laser = transform.gameObject.GetComponent<LineRenderer>();
        laser.SetWidth(0.2f, 0.2f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        pathList.Clear();
        checkDest();
        //laserIsOn = (state == 4) ? true : false;
        //DrawLaser();
        
        //Debug.Log(rayCheck(ship.transform.position, target.transform.position));
        drawPath();
        //UpdateState();
    }

    private void UpdateState()
    {
        switch (state)
        {
            case 0:
                {
                    if (Time.time > timer)
                    {
                        timer = Time.time + 1f;
                        state = 1;
                        if (Vector3.Distance(origin, ship.transform.position) > 50f) state = 5;
                    }

                }; break;
            case 1:
                {
                    FindTarget();
                    if (target == null) state = 0;
                    else if (target.name == "SpaceShip") state = 4;
                    else
                    {
                        destination = ship.transform.position;
                        state = 2;
                    }
                }; break;
            case 2:
                {
                    if (destination != null)
                    {
                        if (Vector3.Distance(ship.transform.position, destination) > 10f) state = 3;
                        else PatrolArea();
                    }
                    else state = 0;

                }; break;
            case 3:
                {
                    if (destination != null) GoToDestination();
                    else state = 0;

                    FindTarget();
                    if (target != null)
                        if (target.name == "SpaceShip") state = 4;
                }; break;
            case 4:
                {
                    if (Vector3.Distance(target.transform.position, ship.transform.position) > 50f)
                    {
                        target = null;
                        state = 1;
                    }
                    else AttackTarget();
                }; break;
            case 5:
                {
                    destination = origin;
                    state = 3;
                }; break;
        }

    }

    private void FindTarget()
    {
        if (Vector3.Distance(player.transform.position, ship.transform.position) < 50f)
            target = player;
        else
            target = null;
    }

    private void PatrolArea()
    {

    }

    private void GoToDestination()
    {
        ship.transform.position = Vector3.MoveTowards(ship.transform.position,destination, 5f * Time.deltaTime);
        ship.transform.LookAt(destination);
    }

    private void AttackTarget()
    {
        DrawLaser();
        //ship.transform.position = Vector3.MoveTowards(ship.transform.position, target.transform.position, 5f * Time.deltaTime);
        //ship.transform.LookAt(target.transform.position);
        ship.transform.rotation = Quaternion.RotateTowards(ship.transform.rotation, Quaternion.LookRotation(target.transform.position - ship.transform.position), Time.deltaTime * 32f);
    }


    private bool rayCheck(Vector3 startPos, Vector3 destPos)
    {
        Ray pathObject = new Ray(startPos, startPos + (destPos - startPos));
        
        bool hit = Physics.Raycast(pathObject, out pathObjectHitInfo);
        if (hit)
            return (!(pathObjectHitInfo.transform.gameObject == target) && (Vector3.Distance(startPos, destPos) > Vector3.Distance(startPos, pathObjectHitInfo.transform.gameObject.transform.position)));
        else
            return false;
        // || (Vector3.Distance(startPos, destPos) > Vector3.Distance(startPos, pathObjectHitInfo.transform.gameObject.transform.position))
    }
    private void checkDest(){
        //only run this function once to get path.
        if (rayCheck(ship.transform.position,target.transform.position))
        {//an obstacle was detected

            //generates new path points to avoid obstacle
            //checks if path to the newdest is ok
            checkPath(ship.transform.position);
            //if one is ok then adds newdest to pathlist
        }
        int failSafe = 10;
        int counter = 0;
        if(pathList.Count > 0){//if there are any path points in the list
            //while (rayCheck(pathList[pathList.Count - 1], target.transform.position))
            while (rayCheck(pathList[pathList.Count - 1], target.transform.position) && (counter < failSafe))//for every (shouldnt we check the first path instead of the last one?)
            {
                checkPath(pathList[pathList.Count - 1]);
                Debug.Log("inside while");
                counter++;
            }
        }

        //int temp = pathList.FindLast();
        pathList.Add(target.transform.position);
        //pathList.Reverse();

        //add path points to patrolpath in reverse order
        //PatrolPathList.Add(target.transform.position);
        //for (int i = pathList.Count; i > 0; i--)
        //{
        //    PatrolPathList.Add(pathList[i]);
        //    pathList.RemoveAt(i);
        //}
        
    }

    private Vector3 generateNewPoint(Vector3 start)
    {
        //take pathObjectHitInfo.transform.gameObject;
        //maths to add distance perpendicular to raydirection
        float radius = Vector3.Distance(pathObjectHitInfo.point, pathObjectHitInfo.transform.gameObject.transform.position);
        Vector3 perp = Vector3.Cross((target.transform.position - start), Vector3.up).normalized;
        return (pathObjectHitInfo.transform.gameObject.transform.position +( perp * radius *2f));
        //should return a point that is 15 units 
    }

    private void checkPath(Vector3 start)
    {
        int checkPathIndex = 0;//every fail adds to this
        int failSafe = 10;
        int counter = 0;

        do
        {
            newDest = generateNewPoint(start);
            checkPathIndex++;
            counter++;
        } while (rayCheck(start, newDest) && (counter < failSafe)); // ADDED FAILSAFE
        //} while (rayCheck(start, newDest) && (pathObjectHitInfo.distance < Vector3.Distance(start, newDest)));
        //} while (((rayCheck(start, newDest))&&(checkPathIndex<4)) && counter < failSafe);
        //
        if (checkPathIndex > 4)
        {
            ;//state=destroyOriginalAsteroid;
            //re-raycasts and destroys initial object that was in it's way
        }
        else
        {
            pathList.Add(newDest);
            ;//list.add(newDest);
        }
    }

    private void drawPath()
    {
        
            laser.SetVertexCount(pathList.Count +1);
            var points = new Vector3[pathList.Count + 1];

            points[0] = ship.transform.position;
            points[1] = ship.transform.position;
            for (int i = 0; i < pathList.Count; i++)
            {
                points[i+1] = pathList[i];
            }

            laser.SetPositions(points);
    }     
    

    [SerializeField]
    private GameObject laserGo;
    [SerializeField]
    private LineRenderer laser;
    [SerializeField]
    private Material activeLaserColor;
    [SerializeField]
    private Material idleLaserColor;

    private GameObject laserTarget = null;

    RaycastHit hitInfo;
    Ray detectObject;
    bool hit = false;
    float laserRange = 25f;

    private void DrawLaser()
    {
        if (laserIsOn) // if the laser is on
        {
            // find target -------------------
            detectObject = new Ray(laserGo.transform.position, laserGo.transform.right * laserRange);
            hit = Physics.Raycast(detectObject, out hitInfo);

            if (hit && Vector3.Distance(laserGo.transform.position, hitInfo.point) < laserRange)
                laserTarget = hitInfo.transform.gameObject;
            else
                laserTarget = null;
            //---------------------------------

            laser.SetPosition(0, laserGo.transform.position);                                                                      // set line start position

            if (laserTarget != null)                                                                                                 // if there is something infront
            {
                if (Vector3.Distance(laserGo.transform.position, hitInfo.point) < laserRange)                           // and in range
                {
                    laser.SetPosition(1, hitInfo.point);                                                                        // set the laser to point to that area
                    laser.GetComponent<Renderer>().material = (laserTarget == player) ? activeLaserColor : idleLaserColor;  // set the color of the laser
                }
            }
            else                                                                                                                // if there is nothing in front
            {
                laser.SetPosition(1, (laserGo.transform.position + laserGo.transform.right * laserRange));                   // set the laser to max range
                laser.GetComponent<Renderer>().material = idleLaserColor;                                                       // and change color to idle
            }

        }
        else                                                                                                                    // if the laser is off, turn off the line
        {
            laser.SetPosition(0, laserGo.transform.position);
            laser.SetPosition(1, laserGo.transform.position);
        }
    }
}

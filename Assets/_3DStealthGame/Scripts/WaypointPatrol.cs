using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public Transform[] waypoints;

    private Rigidbody m_RigidBody;
    //Tracks waiting timer
    private float waitTimer = 0f;
    //Amount of time that the enemy will wait
    private float waitTime;
    int m_CurrentWaypointIndex;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        waitTime = Random.Range(1f, 3f);
    }

    void FixedUpdate()
    {
        Transform currentWaypoint = waypoints[m_CurrentWaypointIndex];
        Vector3 currentToTarget = currentWaypoint.position - m_RigidBody.position;

        //If the enemy is close enough to the target...
        if (currentToTarget.magnitude < 0.1f)
        {
            //Every frame adds to the timer
            waitTimer += Time.deltaTime;
            //If the enemy as waited long enough...
            if (waitTimer >= waitTime)
            {
                int nextIndex;
                //Do while loop chooses a new waypoint, excluding the current one
                do
                {
                    nextIndex = Random.Range(0, waypoints.Length);
                } while (nextIndex == m_CurrentWaypointIndex);
                //New waypoint
                m_CurrentWaypointIndex = nextIndex;
                //Resets timer and makes new random wait time
                waitTimer = 0f;
                waitTime = Random.Range(1f, 3f);
            }
            return;
        }
        Quaternion forwardRotation = Quaternion.LookRotation(currentToTarget);
        m_RigidBody.MoveRotation(forwardRotation);
        m_RigidBody.MovePosition(m_RigidBody.position + currentToTarget.normalized * moveSpeed * Time.deltaTime);
    }
}

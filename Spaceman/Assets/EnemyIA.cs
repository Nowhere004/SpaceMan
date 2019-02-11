using Pathfinding;
using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyIA : MonoBehaviour {
    //what to chase?
    public Transform target;
    //how many times each second we will update our path
    public float UpdateRate = 2f;

    private Seeker seeker;
    private Rigidbody2D rb;

    //The Calculated Path
    public Path path;
    //The IA's speed per second
    public float speed = 300f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded=false;
    //Max Distance from the IA to a waypoint for it to continue to the next waypoint
    public float nextWayPointDistance = 3f;

    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingForPlayer = false;




    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        if (target==null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }
        //
        seeker.StartPath(transform.position,target.position,OnPathComplate);
        StartCoroutine(UpdatePath());
    }
    IEnumerator SearchForPlayer()
    {
        GameObject sResult= GameObject.FindGameObjectWithTag("Player");
        if (sResult == null) { 
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else
        {
            target = sResult.transform;
            searchingForPlayer = false;
            yield return false;
            StartCoroutine(UpdatePath());
        }
    }

    public void OnPathComplate(Path p)
    {
        Debug.Log("We Got a Path.Did it Have an Error"+p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;

        }
    }
    IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield return false;
        }else
        seeker.StartPath(transform.position, target.position, OnPathComplate);
        yield return new WaitForSeconds(1f/ UpdateRate) ;
        StartCoroutine(UpdatePath());

    }

    void FixedUpdate()
    {
        if (target == null)
        {
            //TODO: Insert a player search here
            return;
        }

        //TODO: Always Look At The Player
        if (path==null)
            return;
        if (currentWaypoint>=path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;
            Debug.Log("End of Path Reached");
            pathIsEnded = true;
            return;

        }
        pathIsEnded = false;
        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed*Time.fixedDeltaTime;

        //Move the IA
        rb.AddForce(dir,fMode);
        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist<nextWayPointDistance)
        {
            currentWaypoint++;
            return; 

        }

    }

    
}

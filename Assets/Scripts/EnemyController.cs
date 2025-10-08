using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [Header("Stats")]
    public float walkingSpeed;
    public float runningSpeed;
    public float hitpoints;
    public float damage;
    public float fov;
    public float sightRange;
    public float attackRange; //range enemy can reach player
    public float attackRangeOffset;  //how much closer then max range should enemy get before attacking

    private float speed;
    private bool stopSearchQueued;
    private bool newRoamDestQueued;

    public NavMeshAgent nav;
    private Transform currentTarget;
    private Vector3 lastSeen;
    private Vector3 currentRoamTarget;

    //for debug
    public PlayerController player;
    public Canvas canvas;
    public TextMeshProUGUI text;


    private EnemyState state;
    public enum EnemyState
    {
        Roaming,
        Chasing,
        Attacking,
        Searching
    }


    private void Start()
    {
        state = EnemyState.Roaming;
        //rb = GetComponent<Rigidbody>();
        //rb.freezeRotation = true;

        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {

        StateHandler();
        debug();
       
    }

    private void StateHandler()
    {
        if (state == EnemyState.Roaming)
        {
            speed = walkingSpeed;
            Roam();
        }

        else if (state == EnemyState.Chasing)
        {
            speed = runningSpeed;
            Chasing();
        }

        else if (state == EnemyState.Attacking)
        {
            speed = runningSpeed;
            Attacking();
        }

        else if (state == EnemyState.Searching)
        {
            speed = walkingSpeed;
            Searching();
        }

        nav.speed = speed;
    }

    private void Roam()
    {
        if ((currentRoamTarget == Vector3.zero || transform.position == currentRoamTarget) && !newRoamDestQueued)
        {
            Invoke(nameof(newRoamDest), Random.Range(1f, 3f));
            newRoamDestQueued = true;
        }
    }

    private void newRoamDest()
    {
        currentRoamTarget = transform.position + new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
        nav.SetDestination(currentRoamTarget);
        newRoamDestQueued = false;
    }

    private void Chasing()
    {
        if (currentTarget == null)
        {
            state = EnemyState.Searching;
        }
        else if (Vector3.Distance(transform.position, currentTarget.transform.position) < attackRange - attackRangeOffset)
        {
            state = EnemyState.Attacking;
        }
        else if (currentTarget.transform.position != lastSeen)
        {
            lastSeen = currentTarget.transform.position;
            nav.SetDestination(lastSeen);
        }
        
    }

    private void Attacking()
    {
        if (currentTarget == null)
        {
            state = EnemyState.Searching;
        }
        else if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRange)
        {
            state = EnemyState.Chasing;
        }
        else
        {
            //deals damage / plays quick time event / kills player
        }
    }

    private void Searching()
    {

        if (!stopSearchQueued)
        {
            if (lastSeen != Vector3.zero)
            {
                if (transform.position != lastSeen)
                {
                    nav.SetDestination(lastSeen);
                }
                else
                {
                    //to add lookaround()
                    Invoke(nameof(stopSearch), 5f);
                    stopSearchQueued = true;
                }
            }
            else
            {
                Invoke(nameof(stopSearch), 5f);
                stopSearchQueued = true;
            }
        }
    }

    private void stopSearch()
    {
        state = EnemyState.Roaming;
        stopSearchQueued = false;
        newRoamDestQueued = false;
        currentRoamTarget = Vector3.zero;
        lastSeen = Vector3.zero;
    }

    private IEnumerator FOVRoutine()
    {

        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sightRange, LayerMask.GetMask("Player"));

        if (rangeChecks.Length > 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.position, directionToTarget) < transform.rotation.eulerAngles.y + fov / 2 && !Physics.Raycast(transform.position, directionToTarget, sightRange, LayerMask.GetMask("whatIsGround")))
            {
                currentTarget = target;
                state = EnemyState.Chasing;
            }
            else
            {
                currentTarget = null;
            }
        }
        else
        {
            currentTarget = null;
        }
    }

    private void debug()
    {
        if (player.inDebug)
        {
            string stateString = "";
            switch (state)
            {
                case EnemyState.Roaming:
                    stateString = "Roaming";
                    break;
                case EnemyState.Chasing:
                    stateString = "Chasing";
                    break;
                case EnemyState.Attacking:
                    stateString = "Attacking";
                    break;
                case EnemyState.Searching:
                    stateString = "Searching";
                    break;
            }
            text.text = "State: " + stateString;
            Vector3 dir = canvas.transform.position - player.transform.position;
            canvas.transform.rotation = Quaternion.LookRotation(dir);


            Debug.DrawRay(transform.position, transform.TransformDirection(Quaternion.AngleAxis(fov / 2, Vector3.up) * Vector3.forward) * sightRange, Color.yellow);
            Debug.DrawRay(transform.position, transform.TransformDirection(Quaternion.AngleAxis(-fov / 2,Vector3.up) * Vector3.forward) * sightRange, Color.yellow); 
        }
        else
        {
            text.text = "";
        }
    }
}

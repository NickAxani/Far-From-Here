using System.Collections;
using TMPro;
using UnityEditorInternal;
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
    public float sightRange; // range in front of enemy
    public float soundRange;
    public float radiusSight; //radius around enemy where it can see if player is near
    public float attackRange; //range enemy can reach player
    public float attackRangeOffset;  //how much closer then max range should enemy get before attacking

    private float speed;
    private bool newRoamDestQueued;

    public NavMeshAgent nav;
    private Transform currentTarget;
    private Vector3 lastSeen;
    private Vector3 currentRoamTarget;
    private Coroutine stopSearchCoroutine;
    public GameObject sight;

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
        StartCoroutine(SoundRoutine());
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
        if (lastSeen != Vector3.zero)
        {
            float distanceToLastSeen = Vector3.Distance(transform.position, lastSeen);
            if (distanceToLastSeen > 0.2f)
            {
                nav.SetDestination(lastSeen);

                // If a stopSearch was queued, cancel it because we still have a target
                if (stopSearchCoroutine != null)
                {
                    StopCoroutine(stopSearchCoroutine);
                    stopSearchCoroutine = null;
                }
            }
            else
            {
                // Start lookaround or stop search coroutine if not already running
                if (stopSearchCoroutine == null)
                {
                    stopSearchCoroutine = StartCoroutine(StopSearchAfterDelay(5f));
                }
            }
        }
        else
        {
            // No lastSeen position, stop search after delay if not already queued
            if (stopSearchCoroutine == null)
            {
                stopSearchCoroutine = StartCoroutine(StopSearchAfterDelay(5f));
            }
        }
    }

    private IEnumerator StopSearchAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Only stop search if lastSeen hasn't changed during the wait
        if (Vector3.Distance(transform.position, lastSeen) <= 0.2f)
        {
            StopSearch();
        }

        stopSearchCoroutine = null;
    }

    private void StopSearch()
    {
        state = EnemyState.Roaming;
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

    private IEnumerator SoundRoutine()
    {

        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            SoundCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, sightRange, LayerMask.GetMask("Player"));

        if (rangeChecks.Length > 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, LayerMask.GetMask("whatIsGround")))
            {
                if (distanceToTarget < radiusSight)
                {
                    currentTarget = target;
                    state = EnemyState.Chasing;
                }
                else if (Vector3.Angle(transform.forward, directionToTarget) < fov / 2)
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
        else
        {
            currentTarget = null;
        }
    }
    
    private void SoundCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, soundRange, LayerMask.GetMask("Sound"));

        if (rangeChecks.Length > 0)
        {
            if (state == EnemyState.Roaming || state == EnemyState.Searching)
            {
                lastSeen = rangeChecks[0].transform.position;
                state = EnemyState.Searching;
                print(lastSeen);
            }
            
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

            sight.SetActive(true);
        }
        else
        {
            text.text = "";

            sight.SetActive(false);
        }
    }


}

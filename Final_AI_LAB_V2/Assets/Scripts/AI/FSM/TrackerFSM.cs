using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class TrackerFSM : MonoBehaviour
{
    //STATES
    public enum State { IDLE, FOLOW_SCENT, LOST_SCENT, SEARCH, RETURN };
    private State currentState;

    //agent info
    private NavMeshAgent agent;
    private ZombieAI zombie;  
    private Vector3 startPos;
    public float stopDistance = 1;
    public float searchRadius = 5;
   
    // timers
    float lostScentTimer = 0;
    float lostScentDuration = 2;
    float searchTimer = 0;
    float searchDuration = 3;

    void Start()
    {
        zombie = GetComponent<ZombieAI>();
        agent = GetComponent<NavMeshAgent>();
        startPos = transform.position;
        currentState = State.IDLE;
    }

    void Update()
    {
        if (GlobalBlackboard.Instance.globalAlert)
        {
                agent.SetDestination(GlobalBlackboard.Instance.alertPosition);
        }
        else
        {
            switch (currentState)
            {
                case State.IDLE:
                    Idle();
                    break;
                case State.FOLOW_SCENT:
                    FollowScent();
                    break;
                case State.LOST_SCENT:
                    LostScent();
                    break;
                case State.SEARCH:
                    Search();
                    break;
                case State.RETURN:
                    Return();
                    break;
            }
        }
            
    }
    void ChangeState(State newState)
    {
        //set up and to 0 timers depending on state
        //to exit state
        switch (currentState)
        {
            case State.LOST_SCENT:
                lostScentTimer = 0f;
                break;

            case State.SEARCH:
                searchTimer = 0f;
                break;
        }
        //to enter state
        currentState = newState;
        switch (newState)
        {
            case State.SEARCH:
                searchTimer = 0f;
                SetRandomSearchPoint();
                break;

            case State.LOST_SCENT:
                lostScentTimer = 0f;
                break;
        }
    }
    void Idle()
    {
        if (zombie.blackboard.scentDetected)
        {
            ChangeState(State.FOLOW_SCENT);
            return;
        }
    }
    void FollowScent()
    {
        if (zombie.blackboard.scentDetected == false)
        {
            ChangeState(State.LOST_SCENT);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < stopDistance)
        {
            zombie.blackboard.scentDetected = false;
            ChangeState(State.SEARCH);
        }
        if ((zombie.blackboard.scentPosition - transform.position).magnitude > stopDistance)
        {
            agent.SetDestination(zombie.blackboard.scentPosition);
        }
       
    }

   
    void LostScent()
    {
        lostScentTimer += Time.deltaTime;

        if (zombie.blackboard.scentDetected)
        {
            ChangeState(State.FOLOW_SCENT);
            return;
        }

        if (lostScentTimer >= lostScentDuration)
        {
            ChangeState(State.SEARCH);
        }
    }

    void Search()
    {
        searchTimer += Time.deltaTime;

        if (zombie.blackboard.scentDetected)
        {
            ChangeState(State.FOLOW_SCENT);
            return;
        }

        if ((agent.pathPending == false) && agent.remainingDistance < stopDistance)
        {
            SetRandomSearchPoint();
        }

        if (searchTimer > searchDuration)
        {
            ChangeState(State.RETURN);
        }
    }

    void SetRandomSearchPoint()
    {
        Vector3 randomPoint = zombie.blackboard.scentPosition + Random.insideUnitSphere * searchRadius;
        randomPoint.y = transform.position.y;
        agent.SetDestination(randomPoint);
    }

    void Return()
    {
        if (zombie.blackboard.scentDetected)
        {
            ChangeState(State.FOLOW_SCENT);
            return;
        }
        agent.SetDestination(startPos);

        if ((startPos- transform.position).magnitude < stopDistance)
        {
            ChangeState(State.IDLE);
        }
    }

   
}

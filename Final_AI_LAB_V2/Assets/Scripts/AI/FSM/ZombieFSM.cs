using UnityEngine;
using UnityEngine.AI;




public class ZombieFSM : MonoBehaviour
{
    enum State { Patrol, Alert, Chase, Attack, Return }
    State currentState;

    public Transform[] patrolPoints;
    int currentPatrolIndex;

    NavMeshAgent agent;
    WatcherVision vision;
    Vector3 startPosition;

    //para los materiales
    public Renderer bodyRenderer;
    public Material matNormal;
    public Material matChase;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<WatcherVision>();
        startPosition = transform.position;

        // Material por defecto
        if (bodyRenderer != null && matNormal != null)
            bodyRenderer.material = matNormal;

        currentState = State.Patrol;
        GoToNextPatrolPoint();
        //UpdateStateText();

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
                case State.Patrol:
                    Patrol();
                    break;
                case State.Alert:
                    Alert();
                    break;
                case State.Chase:
                    Chase();
                    break;
                case State.Attack:
                    Attack();
                    break;
                case State.Return:
                    Return();
                    break;
            }

        }
          
    }

    void Patrol()
    {
        if (bodyRenderer != null && matNormal != null)
            bodyRenderer.material = matNormal;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPatrolPoint();

        if (vision.playerInSight)
        {
            currentState = State.Chase;
            //UpdateStateText();
        }

    }

    void Alert()
    {
        // Could be used for temporary searching
    }

    void Chase()
    {
        if (bodyRenderer != null && matChase != null)
            bodyRenderer.material = matChase;

        if (vision.playerInSight)
            agent.SetDestination(vision.lastKnownPosition);
        else
        {
            currentState = State.Return;
            //UpdateStateText();
        }
    }

    void Attack()
    {
        // Optional later
    }

    void Return()
    {
        agent.SetDestination(startPosition);

        if (Vector3.Distance(transform.position, startPosition) < 1f)
            currentState = State.Patrol;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }
}

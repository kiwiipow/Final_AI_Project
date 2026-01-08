using UnityEngine;
using UnityEngine.AI;

public class ListenerFSM : MonoBehaviour
{
    enum State { Idle, Investigate, Alert, Return }
    State currentState;

    NavMeshAgent agent;
    ListenerHearing hearing;
    Vector3 startPosition;

    [Header("Visual Feedback")]
    public Renderer bodyRenderer;
    public Material matNormal;
    public Material matAlert;

    private float alertTimer;
    public float alertDuration = 4f; // tiempo de espera en modo alerta

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        hearing = GetComponent<ListenerHearing>();
        startPosition = transform.position;

        if (bodyRenderer != null && matNormal != null)
            bodyRenderer.material = matNormal;

        currentState = State.Idle;
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
                case State.Idle:
                    Idle();
                    break;
                case State.Investigate:
                    Investigate();
                    break;
                case State.Alert:
                    Alert();
                    break;
                case State.Return:
                    Return();
                    break;
            }

        }
          
    }

    void Idle()
    {
        if (bodyRenderer != null && matNormal != null)
            bodyRenderer.material = matNormal;

        // Si escucha un sonido, ir a investigar
        if (hearing.heardSound)
        {
            agent.SetDestination(hearing.soundPosition);
            currentState = State.Investigate;
        }
    }

    void Investigate()
    {
        if (bodyRenderer != null && matAlert != null)
            bodyRenderer.material = matAlert;

        // Si llega al punto del sonido
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            currentState = State.Alert;
            alertTimer = 0f;
        }
    }

    void Alert()
    {
        alertTimer += Time.deltaTime;
        if (alertTimer > alertDuration)
        {
            currentState = State.Return;
            hearing.heardSound = false;
            GetComponent<ZombieAI>().blackboard.heardSound = false;
        }
    }

    void Return()
    {
        agent.SetDestination(startPosition);
        if (Vector3.Distance(transform.position, startPosition) < 1f)
            currentState = State.Idle;
    }
}

using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("Core References")]
    public Blackboard blackboard = new Blackboard();
    public NavMeshAgent agent;
    public State currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
       
    }

    private void Update()
    {
        if (GlobalBlackboard.Instance.globalAlert)
        {  
                agent.SetDestination(GlobalBlackboard.Instance.alertPosition);

        }
        else if (currentState != null)
        {
            currentState.Update();
        }     
    }
   
    public void ChangeState(State newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        if (currentState != null)
            currentState.Enter();
    }
}

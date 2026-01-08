using UnityEngine;
using UnityEngine.AI;

public class MoveTest : MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.Move(transform.forward * 3f * Time.deltaTime);
    }
}

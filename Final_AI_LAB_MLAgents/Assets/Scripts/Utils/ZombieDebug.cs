using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Transform))]
public class ZombieDebug : MonoBehaviour
{
    private ZombieAI zombieAI;
    private Component fsmComponent; // puede ser ZombieFSM, ListenerFSM, etc.

    void OnDrawGizmos()
    {
        if (zombieAI == null)
            zombieAI = GetComponent<ZombieAI>();

        // Buscar cualquier componente FSM con un campo "currentState"
        if (fsmComponent == null)
        {
            foreach (var comp in GetComponents<MonoBehaviour>())
            {
                if (comp == null) continue;
                var field = comp.GetType().GetField("currentState",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    fsmComponent = comp;
                    break;
                }
            }
        }

        // Dibujar línea de destino si hay NavMeshAgent
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, agent.destination);
        }

#if UNITY_EDITOR
        string stateName = "NoState";

        if (zombieAI != null && zombieAI.currentState != null)
            stateName = zombieAI.currentState.GetType().Name;
        else if (fsmComponent != null)
        {
            var stateField = fsmComponent.GetType().GetField("currentState",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (stateField != null)
            {
                var stateValue = stateField.GetValue(fsmComponent);
                stateName = stateValue != null ? stateValue.ToString() : "NoState";
            }
        }

        Handles.Label(transform.position + Vector3.up * 2f, stateName);
#endif
    }
}


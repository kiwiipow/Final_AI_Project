using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WatcherVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector] public bool playerInSight;
    [HideInInspector] public Vector3 lastKnownPosition;

    private void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        playerInSight = false;

        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (var target in targets)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dist, obstacleMask))
                {
                    playerInSight = true;
                    lastKnownPosition = target.transform.position;
                   GlobalBlackboard.Instance.SetGlobalAlert(target.transform.position);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = playerInSight ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 left = DirFromAngle(-viewAngle / 2);
        Vector3 right = DirFromAngle(viewAngle / 2);

        Gizmos.DrawLine(transform.position, transform.position + left * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRadius);
    }

    Vector3 DirFromAngle(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}

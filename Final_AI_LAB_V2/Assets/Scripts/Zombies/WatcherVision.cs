using UnityEngine;

public class WatcherVision : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool debugLogs = false;
    public bool drawRays = true;

    [HideInInspector] public bool playerInSight;
    [HideInInspector] public Vector3 lastKnownPosition;

    private void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        playerInSight = false;

        // If a LayerMask is left empty in the Inspector it will be zero.
        // Treat zero as "all layers" so detection still works if user didn't set the masks.
        int targetMaskBits = targetMask.value == 0 ? ~0 : targetMask.value;
        int obstacleMaskBits = obstacleMask.value == 0 ? ~0 : obstacleMask.value;

        Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, targetMaskBits);

        if (debugLogs) Debug.Log($"{name} OverlapSphere found {targets.Length} target(s) (targetMask bits: {targetMaskBits})");

        foreach (var target in targets)
        {
            if (target == null) continue;

            // Use elevated origin/target to avoid ray starting inside feet or ground
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 targetPos = target.transform.position + Vector3.up * 0.5f;

            Vector3 dirToTarget = (targetPos - origin).normalized;
            float dist = Vector3.Distance(origin, targetPos);

            float angleToTarget = Vector3.Angle(transform.forward, (target.transform.position - transform.position).normalized);
            if (angleToTarget < viewAngle / 2)
            {
                // Raycast using obstacleMaskBits (if Inspector left empty, will use all layers)
                bool blocked = Physics.Raycast(origin, dirToTarget, dist, obstacleMaskBits);
                if (debugLogs)
                {
                    Debug.Log($"{name} checking '{target.name}': angle={angleToTarget:F1}, dist={dist:F2}, blocked={blocked}");
                }

                if (!blocked)
                {
                    playerInSight = true;
                    lastKnownPosition = target.transform.position;
                    GlobalBlackboard.Instance.SetGlobalAlert(target.transform.position);

                    // break if you only care about first visible target
                    break;
                }
                else if (drawRays)
                {
                    Debug.DrawLine(origin, origin + dirToTarget * dist, Color.red, 0.1f);
                }
            }
            else if (debugLogs)
            {
                Debug.Log($"{name} '{target.name}' is outside view angle ({angleToTarget:F1} deg)");
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

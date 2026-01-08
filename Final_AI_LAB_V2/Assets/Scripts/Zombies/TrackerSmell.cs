
using UnityEngine;

public class TrackerSmell : MonoBehaviour
{
    public float smellRadius = 8f;
    public LayerMask scentMask;

    ZombieAI zombie;

    void Start()
    {
        zombie = GetComponent<ZombieAI>();
    }

    void Update()
    {
        DetectScent();
    }

    void DetectScent()
    {
        //get colliders that are inside the smellradius and correspond to the smell mask to avoid picking other senses
        //put them in array
        Collider[] scentsArray = Physics.OverlapSphere(transform.position, smellRadius, scentMask);

        if (scentsArray.Length == 0)
        {
            zombie.blackboard.scentDetected = false;
            return;
        }
       
        ScentPoint best = null;
        float bestScore = 0f;

        //loop scentarray 
        foreach (var sc in scentsArray)
        {
            ScentPoint sp = sc.GetComponent<ScentPoint>();
            //skip if a object is null or just reached 0 
            if (sp == null || sp.intensity <= 0)
            {
                continue;
            }

            //check for the one with more intensity that is closer
            float distance = Vector3.Distance(transform.position, sc.transform.position);
            float score = sp.intensity / (distance);

            if (score > bestScore)
            {
                bestScore = score;
                best = sp;
            }
        }
        //set position and store in blackboard
        if (best != null)
        {
            zombie.blackboard.scentDetected = true;
            zombie.blackboard.scentIntensity = best.intensity;
            zombie.blackboard.scentPosition = best.transform.position;
            GlobalBlackboard.Instance.SetGlobalAlert(best.transform.position);
        }
    }

    //debug
    private void OnDrawGizmosSelected()
    {
        // Prevent errors in edit mode
        if (zombie == null)
            zombie = GetComponent<ZombieAI>();

        if (zombie == null || zombie.blackboard == null)
            return;

        // Draw smell radius
        Gizmos.color = zombie.blackboard.scentDetected ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, smellRadius);

        // Draw detected scent point
        if (zombie.blackboard.scentDetected)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(zombie.blackboard.scentPosition, 0.3f);
        }
    }
}


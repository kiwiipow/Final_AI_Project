using UnityEngine;

public class ScentPoint : MonoBehaviour
{
    public float intensity;
    public float decayRate;

    void Update()
    {
        //decrease intensity over time
        intensity -= decayRate * Time.deltaTime;
        intensity = Mathf.Max(0, intensity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1);
    }
}



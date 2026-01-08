using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ListenerHearing : MonoBehaviour
{
    [Header("Hearing Settings")]
    public float hearingRadius = 10f;
    public LayerMask soundMask; // capa donde estarán los sonidos
    public bool heardSound;
    public Vector3 soundPosition;

    private float soundTimer = 0f;
    public float soundMemoryTime = 3f; // cuánto tiempo recuerda el sonido

    void Update()
    {
        DetectSound();

        // si escuchó un sonido recientemente, contar tiempo hasta que lo olvide
        if (heardSound)
        {
            soundTimer += Time.deltaTime;
            if (soundTimer > soundMemoryTime)
            {
                heardSound = false;
                soundTimer = 0f;
            }
        }
    }

    void DetectSound()
    {
        Collider[] sounds = Physics.OverlapSphere(transform.position, hearingRadius, soundMask);

        if (sounds.Length > 0)
        {
            // Detecta el sonido más cercano
            Collider closest = sounds[0];
            float minDist = Vector3.Distance(transform.position, closest.transform.position);

            foreach (var s in sounds)
            {
                float dist = Vector3.Distance(transform.position, s.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = s;
                }
            }

            heardSound = true;
            soundPosition = closest.transform.position;

            // Guardar en el Blackboard local
            var zombie = GetComponent<ZombieAI>();
            if (zombie != null)
            {
                zombie.blackboard.heardSound = true;
                zombie.blackboard.soundPosition = soundPosition;
                GlobalBlackboard.Instance.SetGlobalAlert(soundPosition);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = heardSound ? Color.cyan : Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}

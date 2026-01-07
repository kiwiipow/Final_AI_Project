using UnityEngine;

public class PlayerSoundEmitter : MonoBehaviour
{
    [Header("Sound Settings")]
    public float soundRadius = 5f;
    public float soundDuration = 1f;
    public LayerMask soundLayer; // capa usada por los zombies para detectar sonido
    public KeyCode emitKey = KeyCode.E;

    [Header("Debug")]
    public Color gizmoColor = Color.cyan;

    private void Update()
    {
        if (Input.GetKeyDown(emitKey))
        {
            EmitSound();
        }
    }

    void EmitSound()
    {
        // Crear un GameObject temporal que representará el sonido
        GameObject soundObj = new GameObject("SoundEvent");
        soundObj.transform.position = transform.position;

        // Añadir un collider que los zombies detectarán
        SphereCollider soundCollider = soundObj.AddComponent<SphereCollider>();
        soundCollider.isTrigger = true;
        soundCollider.radius = soundRadius;

        // Asignar capa
        soundObj.layer = LayerMask.NameToLayer(LayerMaskLayerName());

        // Destruirlo después de un tiempo
        Destroy(soundObj, soundDuration);
    }

    // Devuelve el nombre de la capa según el LayerMask asignado
    string LayerMaskLayerName()
    {
        int mask = soundLayer.value;
        for (int i = 0; i < 32; i++)
        {
            if ((mask & (1 << i)) != 0)
                return LayerMask.LayerToName(i);
        }
        return "Default";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, soundRadius);
    }
}

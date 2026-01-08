using UnityEngine;

[System.Serializable]
public class Blackboard
{
    // Vision
    public bool playerInSight = false;
    public Vector3 lastKnownPosition = Vector3.zero;

    // Hearing
    public bool heardSound = false;
    public Vector3 soundPosition = Vector3.zero;

    // Smell 
    //write from the tracker smell and read from fsm
    public bool scentDetected = false;
    public float scentIntensity = 0f;
    public Vector3 scentPosition = Vector3.zero;

    // Estado general
    public bool isAlerted = false;

    // Limpieza
    public void ResetPerceptions()
    {
        playerInSight = false;
        heardSound = false;
        scentDetected = false;
    }
}

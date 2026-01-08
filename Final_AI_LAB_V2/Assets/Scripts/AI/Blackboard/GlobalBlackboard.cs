using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GlobalBlackboard : MonoBehaviour
{
    public static GlobalBlackboard Instance; // Singleton simple

    [Header("Global Alert Data")]
    public bool globalAlert = false;
    public Vector3 alertPosition = Vector3.zero;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void SetGlobalAlert(Vector3 position)
    {
        globalAlert = true;
        alertPosition = position;
       
    }

    public void ClearAlert()
    {
        globalAlert = false;
    }
}

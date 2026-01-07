using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LeaderZombie : MonoBehaviour
{

    private NavMeshAgent agent;
    public float alertDuration = 5;
    private float alertTimer = 0;
    void Awake()
    {
        
        agent = GetComponent<NavMeshAgent>();
       
    }

    void Update()
    {
        // checks if enough time have pased since alert and clears it
        if (GlobalBlackboard.Instance.globalAlert)
        {
            alertTimer += Time.deltaTime;

            agent.SetDestination(GlobalBlackboard.Instance.alertPosition);

           
            if (alertTimer >= alertDuration)
            {
                GlobalBlackboard.Instance.ClearAlert();
                alertTimer = 0;
            }
        }
    }
   
}

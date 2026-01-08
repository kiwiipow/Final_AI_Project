using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.AI;

public class EnemyAgent : Agent
{
    [Header("References")]
    public Transform player;
    public EnemyAgent otherAgent;
    public Transform environmentParent;

    [Header("Components")]
    private NavMeshAgent navAgent;

    [Header("Settings")]
    public float moveSpeed = 3.5f;
    public float turnSpeed = 180f;
    public float episodeTimeout = 60f;

    private float episodeTimer;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        navAgent.speed = moveSpeed;

        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        // Reset timer
        episodeTimer = 0f;

        // Reset agent position and rotation
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
        navAgent.velocity = Vector3.zero;
        navAgent.Warp(transform.position);

        // Optional: Randomize starting positions slightly
        Vector3 randomOffset = new Vector3(
            Random.Range(-2f, 2f),
            0f,
            Random.Range(-2f, 2f)
        );
        transform.position += randomOffset;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Player position relative to agent (3 values)
        Vector3 playerLocalPos = transform.InverseTransformPoint(player.position);
        sensor.AddObservation(playerLocalPos);

        // Player forward direction (3 values)
        sensor.AddObservation(transform.InverseTransformDirection(player.forward));

        // Distance to player (1 value)
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        sensor.AddObservation(distToPlayer);

        // Teammate position relative to agent (3 values)
        Vector3 otherLocalPos = transform.InverseTransformPoint(otherAgent.transform.position);
        sensor.AddObservation(otherLocalPos);

        // Teammate forward direction (3 values)
        sensor.AddObservation(transform.InverseTransformDirection(otherAgent.transform.forward));

        // Self velocity (3 values)
        sensor.AddObservation(navAgent.velocity);

        // Total: 15 observations
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Update timer
        episodeTimer += Time.fixedDeltaTime;

        // Get actions
        float moveAction = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float turnAction = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Apply rotation
        transform.Rotate(Vector3.up, turnAction * turnSpeed * Time.fixedDeltaTime);

        // Apply movement
        Vector3 moveDir = transform.forward * moveAction * moveSpeed * Time.fixedDeltaTime;
        navAgent.Move(moveDir);

        // --- Rewards ---

        // Small time penalty to encourage speed
        AddReward(-0.001f);

        // Reward for getting closer to player
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer < 5f)
        {
            AddReward(0.005f * (5f - distToPlayer) / 5f);
        }

        // Reward for coordinating with teammate (flanking)
        RewardFlanking();

        // Episode timeout
        if (episodeTimer >= episodeTimeout)
        {
            AddReward(-0.5f); // Penalty for timeout
            EndEpisode();
            otherAgent.EndEpisode();
        }
    }

    private void RewardFlanking()
    {
        if (otherAgent == null) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector3 toPlayerOther = (player.position - otherAgent.transform.position).normalized;

        // Calculate angle between the two agents relative to player
        float dot = Vector3.Dot(toPlayer, toPlayerOther);

        // Reward for being on opposite sides (flanking)
        if (dot < -0.3f)
        {
            AddReward(0.01f);
        }

        // Extra reward for perfect flanking (opposite sides)
        if (dot < -0.7f)
        {
            AddReward(0.02f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Big reward for catching player
            AddReward(2.0f);

            // Bonus if teammate is close (coordinated ambush)
            float teammateDistance = Vector3.Distance(
                otherAgent.transform.position,
                player.position
            );
            if (teammateDistance < 3f)
            {
                AddReward(1.0f);
            }

            EndEpisode();
            otherAgent.EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        // WASD or IJKL controls
        continuousActions[0] = Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.W) ? 1f :
                               Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.S) ? -1f : 0f;
        continuousActions[1] = Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.A) ? -1f :
                               Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.D) ? 1f : 0f;
    }
}



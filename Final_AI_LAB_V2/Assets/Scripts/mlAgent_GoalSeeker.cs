using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
public class mlAgent_GoalSeeker : Agent
{
    [SerializeField] private Transform _goal;
    [SerializeField] private Transform _agent;
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed = 180f;

    [SerializeField] private float _rayLength = 2.0f; // How far the agent “sees”
    private Vector3[] _rayDirections;

    private int _currentEpisode = 0;
    private float _cumulativeReward = 0;

    [SerializeField] private Vector3 fixedGoalPosition = new Vector3(12f, 0.3f, 0f);

    public override void Initialize()
    {

        // Ray directions relative to agent forward
        _rayDirections = new Vector3[5];
        _rayDirections[0] = Vector3.forward; // forward
        _rayDirections[1] = Quaternion.Euler(0, 30, 0) * Vector3.forward; // forward-right
        _rayDirections[2] = Quaternion.Euler(0, -30, 0) * Vector3.forward; // forward-left
        _rayDirections[3] = Quaternion.Euler(0, 60, 0) * Vector3.forward; // right
        _rayDirections[4] = Quaternion.Euler(0, -60, 0) * Vector3.forward; // left
        _currentEpisode = 0;
      _cumulativeReward = 0f;
    }
    public override void OnEpisodeBegin()
    {
        _currentEpisode++;
        _cumulativeReward = 0f;
        SpawnObjects();
    }

    //THIS ONE IS FOR TRAINING WITH RANDOM GOAL POSITION
    //private void SpawnObjects()
    //{
    //    // Reset agent position every episode
    //    transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

    //    transform.localPosition = new Vector3(0f, 0.3f, 0f);

    //    // Only pick a new goalevery 5 episodes
    //    if (_currentEpisode % 3 == 1)
    //    {
    //        // Randomize direction
    //        float randAngle = Random.Range(0f, 360f);
    //        Vector3 randDir = Quaternion.Euler(0f, randAngle, 0f) * Vector3.forward;

    //        // Randomize distance
    //        float randDistance = Random.Range(10f, 20f);

    //        // Calculate goal position
    //        Vector3 goalPos = transform.localPosition + randDir * randDistance;

    //        // Apply random calculated position to goal
    //        _goal.localPosition = new Vector3(goalPos.x, 0.3f, goalPos.z);
    //    }
    //}
    private void SpawnObjects()
    {
        // Reset agent every episode
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(0f, 0.3f, 0f);

        // Always spawn goal at the same position
        _goal.localPosition = fixedGoalPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //normalize values in a small range
        //goal position
        float goalPosX_norm = _goal.localPosition.x / 5f;
        float goalPosZ_norm = _goal.localPosition.z / 5f;

        //agent position
        float agentPosX_norm = transform.localPosition.x / 5f;
        float agentPosZ_norm = transform.localPosition.z / 5f;

        // agent direction on y axis
        float agentRotation_norm = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        foreach (var dir in _rayDirections)//five observations
        {
            Ray ray = new Ray(transform.position, transform.TransformDirection(dir));
            if (Physics.Raycast(ray, out RaycastHit hit, _rayLength))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    sensor.AddObservation(hit.distance / _rayLength); 
                }
                else
                {
                    sensor.AddObservation(1f); 
                }
            }
            else
            {
                sensor.AddObservation(1f); 
            }
        }

        sensor.AddObservation(goalPosX_norm);
        sensor.AddObservation(goalPosZ_norm);
        sensor.AddObservation(agentPosX_norm);
        sensor.AddObservation(agentPosZ_norm);
        sensor.AddObservation(agentRotation_norm);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;

        if(Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = 3;
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //movement logic
        MoveAgent(actions.DiscreteActions);

        //STEP PENALTY
        AddReward(-2f / MaxStep);

        _cumulativeReward = GetCumulativeReward();
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];
        switch (action)
        {
            case 1://move forward
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 2://Rotate left
                transform.Rotate(0f, -_rotationSpeed * Time.deltaTime, 0f);
                break;
            case 3://Rotate right
                transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Goal"))
        {
            GoalReached();
        }
    }

    private void GoalReached()
    {
        //reward for reaching goal
        AddReward(1.0f);
        _cumulativeReward = GetCumulativeReward();

        EndEpisode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.1f);
        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //penalize continuous wall bumping
            AddReward(-0.08f * Time.fixedDeltaTime);
        }

    }

}

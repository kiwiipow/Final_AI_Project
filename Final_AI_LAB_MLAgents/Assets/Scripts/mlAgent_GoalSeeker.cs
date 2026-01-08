using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.UIElements;
public class mlAgent_GoalSeeker : Agent
{
    [SerializeField] private Transform _goal;
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed = 180f;
    //[SerializeField] private Renderer _renderer;

    private int _currentEpisode = 0;
    private float _cumulativeReward = 0;
    public override void Initialize()
    {
      //_renderer = GetComponent<Renderer>();
      _currentEpisode = 0;
      _cumulativeReward = 0f;
    }
    public override void OnEpisodeBegin()
    {
        _currentEpisode++;
        _cumulativeReward = 0f;
        //_renderer.material.color = Color.blue;

        SpawnObjects();
    }
    private void SpawnObjects()
    {
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(0f, 0.3f, 0f);

        //randomize direction
        float randAngle = Random.Range(0f, 360f);
        Vector3 randDir = Quaternion.Euler(0f, randAngle, 0f) * Vector3.forward;

        //randomize distance
        float randDistance = Random.Range(1f, 2.5f);

        //calc goal position
        Vector3 goalPos = transform.localPosition + randDir * randDistance;

        //aply rand calculated pos to goal
        _goal.localPosition = new Vector3(goalPos.x, 0.3f, goalPos.z);
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

        sensor.AddObservation(goalPosX_norm);
        sensor.AddObservation(goalPosZ_norm);
        sensor.AddObservation(agentPosX_norm);
        sensor.AddObservation(agentPosZ_norm);
        sensor.AddObservation(agentRotation_norm);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //movement logic
        MoveAgent(actions.DiscreteActions);

        //small penalty for taking steps
        AddReward(-2f / MaxStep);

        //update total reward for current episode
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
        //big reward for reaching goal
        AddReward(1.0f);
        _cumulativeReward = GetCumulativeReward();

        EndEpisode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.05f);

            //if(_renderer != null)
            //{
            //    _renderer.material.color = Color.red;
            //}
        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //penalize a bit if continuous wall bumping
            AddReward(-0.01f * Time.fixedDeltaTime);
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        //reset agent color
        if (collision.gameObject.CompareTag("Wall"))
        {
            //if (_renderer != null)
            //{
            //    _renderer.material.color = Color.blue;
            //}
        }
    }
}

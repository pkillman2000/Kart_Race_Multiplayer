using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private Circuit _circuit;
    private Drive _driveScript;

    [SerializeField]
    private float _steeringSensitivity = 0.01f;
    [SerializeField]
    private float _accelerationSensitivity = 0.1f;
    [SerializeField]
    private float _brakingSensitivity = 0.8f;
    private Vector3 _target;
    private Vector3 _nextTarget;
    private float _totalDistanceToTarget;
    int _currentWaypoint = 0;
    int _nextWayPoint = 0;

    
    private void Start()
    {
        _driveScript = GetComponent<Drive>();
        if(_driveScript == null)
        {
            Debug.LogError("Drive script is Null!");
        }

        _target = _circuit.GetWaypointPosition(_currentWaypoint);
        _nextTarget = _circuit.GetWaypointPosition(_currentWaypoint + 1);
        _totalDistanceToTarget = Vector3.Distance(_target, _driveScript.GetRigidBody().transform.position);
    }

    private void Update()
    {
        Vector3 localTarget = _driveScript.GetRigidBody().transform.InverseTransformPoint(_target); // Get Vector3 between _target and vehicle
        Vector3 nextLocalTarget = _driveScript.GetRigidBody().transform.InverseTransformPoint(_nextTarget);

        float distanceToTarget = Vector3.Distance(_target, _driveScript.GetRigidBody().transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * _steeringSensitivity, -1, 1) * Mathf.Sign(_driveScript.GetCurrentSpeed());

        float distanceFactor = distanceToTarget / _totalDistanceToTarget; // inverse percentage of how close to target
        float speedFactor = _driveScript.GetCurrentSpeed()/_driveScript.GetMaxSpeed(); // percentage of current vs max speed

        float accel = Mathf.Lerp(_accelerationSensitivity, 1, distanceFactor);
        /*
         * In the Lerp, -1 - Mathf.Abs(nextTargetAngle) will increase braking distance based on the
         * angle to the waypoint AFTER the current target.
         * The 1 + speedFactor will increase the amount of braking based on speed.
         * 1 - distanceFactor will determine how far away from the current target the brakes
         * will be activated.
         */
        float brake = Mathf.Lerp((-1 - Mathf.Abs(nextTargetAngle)) * _brakingSensitivity, 1 + speedFactor, 1-distanceFactor);

        _driveScript.Go(accel, steer, brake);

        if (distanceToTarget < 4) // Threshold - Make this larger if car starts to circle waypoint
        {
            _currentWaypoint++;
            if (_currentWaypoint >= _circuit.GetWaypointsLength())
            {
                _currentWaypoint = 0;
            }
            _target = _circuit.GetWaypointPosition(_currentWaypoint);

            if (_currentWaypoint + 1 >= _circuit.GetWaypointsLength())
            {
                _nextWayPoint = 0;
            }
            else
            {
                _nextWayPoint = _currentWaypoint + 1;
            }

            _nextTarget = _circuit.GetWaypointPosition(_nextWayPoint);
            _totalDistanceToTarget = Vector3.Distance(_target, _driveScript.GetRigidBody().transform.position);
        }
    }
}

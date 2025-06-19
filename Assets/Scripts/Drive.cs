using UnityEngine;

public class Drive : MonoBehaviour
{
    [SerializeField]
    private WheelCollider[] _wheelColliders;
    [SerializeField]
    private GameObject[] _wheelMeshes;
    [SerializeField]
    private float _torque = 200f;
    [SerializeField]
    private float _maxBrakeTorque = 500f;
    [SerializeField]
    private float _maxSteerAngle = 30.0f;

    private void Start()
    {
    }
    
    private void Update()
    {
        float acceleration = Input.GetAxis("Vertical");
        float steeringAngle = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump"); // Spacebar
        Go(acceleration, steeringAngle, brake);
    }

    public void Go(float acceleration, float steeringAngle, float brake)
    {
        Quaternion quat;
        Vector3 position;

        acceleration = Mathf.Clamp(acceleration, -1, 1);
        steeringAngle = Mathf.Clamp(steeringAngle, -1, 1) * _maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * _maxBrakeTorque;
        float thrustTorque = acceleration * _torque;

        // Affect each tire
        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            // Only front tires steer
            if (_wheelColliders[i].gameObject.tag == "Steer Tire")
            {
                _wheelColliders[i].steerAngle = steeringAngle;
            }

            // Brake
            if (_wheelColliders[i].gameObject.tag == "Brake Tire")
            {
                _wheelColliders[i].brakeTorque = brake;
                Debug.Log("Brake: " + brake);
            }

            // Move
            if (brake == 0)
            {
                _wheelColliders[i].motorTorque = thrustTorque;
            }

            // Move  and rotate tire mesh - this does NOT cause rotation
            _wheelColliders[i].GetWorldPose(out position, out quat);
            _wheelMeshes[i].transform.position = position;
            _wheelMeshes[i].transform.rotation = quat;
        }
    }
}

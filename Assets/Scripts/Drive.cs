using UnityEngine;

public class Drive : MonoBehaviour
{
    [Header("Vehicle Mechanics")]
    // Vehicle Mechanics
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
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _maxSpeed = 200f;

    [Header("Skidding")]
    // Skidding Sound
    [SerializeField]
    private AudioSource _skidSound;

    // Skid Marks
    [SerializeField]
    private Transform _skidTrailPrefab;
    private Transform[] _skidTrails = new Transform[4];

    // Smoke VFX
    [SerializeField]
    private ParticleSystem _smokeVFXPrefab;
    private ParticleSystem[] _skidSmoke = new ParticleSystem[4];

    // Brake Lights
    [SerializeField]
    private GameObject _brakeLights;

    // Engine SFX
    [SerializeField]
    private AudioSource _engineSound;
    [SerializeField]
    private float _gearLength = 3;
    private float _currentSpeed { get { return _rigidbody.linearVelocity.magnitude * _gearLength; } }
    [SerializeField]
    private float _lowPitch = 1f;
    [SerializeField]
    private float _highPitch = 6f;
    [SerializeField]
    private int _numGears = 5;
    private float _rpm;
    private int _currentGear = 1;
    private float _currentGearPercentage;


    private void Start()
    {
        for(int i =0; i < 4; i++)
        {
            _skidSmoke[i] = Instantiate(_smokeVFXPrefab);
            _skidSmoke[i].Stop();
        }

        _brakeLights.SetActive(false);
    }
    
    private void Update()
    {
        float acceleration = Input.GetAxis("Vertical");
        float steeringAngle = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump"); // Spacebar
        Go(acceleration, steeringAngle, brake);
        
        CheckForSkid();
        CalculateEngineSound();
    }

    public void Go(float acceleration, float steeringAngle, float brake)
    {
        Quaternion quat;
        Vector3 position;

        acceleration = Mathf.Clamp(acceleration, -1, 1);
        steeringAngle = Mathf.Clamp(steeringAngle, -1, 1) * _maxSteerAngle;
        brake = Mathf.Clamp(brake, 0, 1) * _maxBrakeTorque;
        
        float thrustTorque = 0;
        if (_currentSpeed < _maxSpeed) // Limit top speed of car
        {
            thrustTorque = acceleration * _torque;
        }


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
            }
            // Brake Lights
            if(brake != 0)
            {
                _brakeLights.SetActive(true);
            }
            else
            {
                _brakeLights.SetActive(false);
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

    // Play soundeffect if car is skidding
    private void CheckForSkid()
    {
        int numSkidding = 0;

        for(int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            _wheelColliders[i].GetGroundHit(out wheelHit);

            if(Mathf.Abs(wheelHit.forwardSlip) >= 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) >= 0.4f) // Wheel is skidding
            {
                numSkidding++;
                if(!_skidSound.isPlaying)
                {
                    _skidSound.Play();                    
                }
                // Set position to bottom of tire
                _skidSmoke[i].transform.position = _wheelColliders[i].transform.position - _wheelColliders[i].transform.up * _wheelColliders[i].radius;
                _skidSmoke[i].Emit(1);
            }
            else // Wheel is not skidding
            {
            }
        }

        if(numSkidding == 0 && _skidSound.isPlaying) // No wheel skidding
        {
            _skidSound.Stop();
        }
    }

    private void CalculateEngineSound()
    {
        float gearPercentage = (1 / (float)_numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * _currentGear, gearPercentage * (_currentGear + 1), Mathf.Abs(_currentSpeed/_maxSpeed));

        _currentGearPercentage = Mathf.Lerp(_currentGearPercentage, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = _currentGear / (float)_numGears;
        _rpm = Mathf.Lerp(gearNumFactor, 1, _currentGearPercentage);

        float speedPercentage = Mathf.Abs(_currentSpeed/_maxSpeed);
        float upperGearMax = (1 / (float)_numGears) * (_currentGear + 1);
        float downGearMax = (1/(float)_numGears) * _currentGear;

        if (_currentGear > 0 && speedPercentage < downGearMax) // Shift to lower gear
        {
            _currentGear--;
        }

        if (_currentGear < (_numGears - 1) && speedPercentage > upperGearMax) // Shift to higher gear
        {
            _currentGear++;
        }

        float pitch = Mathf.Lerp(_lowPitch, _highPitch, _rpm);
        _engineSound.pitch = Mathf.Min(_highPitch, pitch) * 0.25f;
    }
}

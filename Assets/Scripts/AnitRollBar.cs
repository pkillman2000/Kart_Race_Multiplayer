using UnityEngine;

public class AnitRollBar : MonoBehaviour
{
    [SerializeField]
    private float _antiRoll = 5000f;
    [SerializeField]
    private WheelCollider _wheelLeftFront;
    [SerializeField]
    private WheelCollider _wheelRightFront;
    [SerializeField]
    private WheelCollider _wheelLeftBack;
    [SerializeField]
    private WheelCollider _wheelRightBack;
    private Rigidbody _rigidBody;
    [SerializeField]
    private GameObject _centerOfMass;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        if (_rigidBody == null)
        {
            Debug.LogError("Rigidbody is Null!");            
        }
        _rigidBody.centerOfMass = _centerOfMass.transform.localPosition;
    }
    
    private void FixedUpdate()
    {
        GroundWheels(_wheelLeftFront, _wheelRightFront);
        GroundWheels(_wheelLeftBack, _wheelRightBack);
    }

    private void GroundWheels(WheelCollider WL, WheelCollider WR)
    {
        WheelHit wheelHit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WL.GetGroundHit(out wheelHit);
        if(groundedL)
        {
            travelL = (-WL.transform.InverseTransformPoint(wheelHit.point).y - WL.radius) / WL.suspensionDistance;
        }

        bool groundedR = WR.GetGroundHit(out wheelHit);
        if (groundedR)
        {
            travelR = (-WR.transform.InverseTransformPoint(wheelHit.point).y - WR.radius) / WR.suspensionDistance;
        }

        float antiRollForce = (travelL - travelR) * _antiRoll;

        if(groundedL)
        {
            _rigidBody.AddForceAtPosition(WL.transform.up * -antiRollForce, WL.transform.position);
        }

        if (groundedR)
        {
            _rigidBody.AddForceAtPosition(WR.transform.up * -antiRollForce, WR.transform.position);
        }

    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Drive _driveScript;
    private void Start()
    {
        _driveScript = GetComponent<Drive>();
        if( _driveScript == null )
        {
            Debug.LogError("Drive script is Null!");
        }
    }

    private void Update()
    {
        float acceleration = Input.GetAxis("Vertical");
        float steeringAngle = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump"); // Spacebar
        _driveScript.Go(acceleration, steeringAngle, brake);
    }
}

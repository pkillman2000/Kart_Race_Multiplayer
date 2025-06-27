using System;
using UnityEngine;

public class FlipCar : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private float _lastTimeChecked;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        if( _rigidBody == null )
        {
            Debug.LogError("Rigidbody is Null!");
        }
    }
    
    private void Update()
    {
        if(transform.up.y > 0.5f || _rigidBody.linearVelocity.magnitude > 1f)
        {
            _lastTimeChecked = Time.time;
        }

        if(Time.time > _lastTimeChecked + 3)
        {
            RightCar();
        }
    }

    private void RightCar()
    {
        this.transform.position += Vector3.up; // Move car into the air before flipping over
        this.transform.rotation = Quaternion.LookRotation(this.transform.forward); // Flip car over
    }
}

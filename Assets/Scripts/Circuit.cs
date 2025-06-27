using UnityEngine;

public class Circuit : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _waypoints;

    public void OnDrawGizmos() // Turns off Gizmos when the script holder is not selected
    {
        DrawGizmos(false);
    }

    public void OnDrawGizmosSelected() // Turns on Gizmos when the script holder is selected
    {
        DrawGizmos(true);
    }

    private void DrawGizmos(bool selected)
    {
        if(selected == false)
        {
            return;
        }

        if(_waypoints.Length > 1)
        {
            Vector3 prev = _waypoints[0].transform.position;
            for (int i = 1; i < _waypoints.Length; i++)
            {
                Vector3 next = _waypoints[i].transform.position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, _waypoints[0].transform.position);
        }
    }

    public Vector3 GetWaypointPosition(int waypointIndex)
    {
        return _waypoints[waypointIndex].transform.position;
    }

    public int GetWaypointsLength()
    {
        return _waypoints.Length;
    }
}

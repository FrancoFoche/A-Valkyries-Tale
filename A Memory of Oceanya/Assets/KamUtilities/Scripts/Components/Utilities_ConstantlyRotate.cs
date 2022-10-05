using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities_ConstantlyRotate : MonoBehaviour
{
    public enum Direction {Clockwise, AntiClockwise}

    public Direction direction;
    public Vector3 axisRotationSpeeds;
    // Update is called once per frame
    void Update()
    {
        if (stopped) return;
        
        Vector3 rotation = direction == Direction.Clockwise ? axisRotationSpeeds : -axisRotationSpeeds;
        transform.Rotate(rotation * Time.deltaTime);
    }
    
    private bool stopped;
    public void Stop()
    {
        stopped = true;
    }

    public void Resume()
    {
        stopped = false;
    }
}

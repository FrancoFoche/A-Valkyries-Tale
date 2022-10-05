using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities_SinewaveMovement : MonoBehaviour
{
    public Vector3 axisRanges;
    public float speed = 1;
    public float offset = 0;
    private Vector3 _startPosition;
    
    void Start () 
    {
        _startPosition = transform.localPosition;
    }
 
    void Update()
    {
        if (stopped) return;
        
        float sine = Mathf.Sin((Time.time * speed)+offset);
        transform.localPosition = _startPosition + new Vector3(sine * axisRanges.x, sine * axisRanges.y,  sine * axisRanges.z);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// just a testing class, nothing here makes sense so don't worry about it
/// </summary>
public class TestingClass : MonoBehaviour
{
    public float time;
    public float start;
    public float end;

    public float cur;

    public bool ongoing;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ongoing = true;
            cur = start;
        }

        if (ongoing)
        {
            if(cur < end)
            {
                if (cur >= end - 0.1f)
                {
                    cur = end;
                    ongoing = false;
                }
                else
                {
                    cur = Mathf.Lerp(cur, end, time * Time.deltaTime);
                    Debug.Log(cur);
                }
            }
            else
            {
                if (cur <= end + 0.1f)
                {
                    cur = end;
                    ongoing = false;
                }
                else
                {
                    cur = Mathf.Lerp(cur, end, time * Time.deltaTime);
                    Debug.Log(cur);
                }
            }
            
        }
    }
}

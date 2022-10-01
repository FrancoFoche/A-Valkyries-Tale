using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GenericInputDetection : MonoBehaviour
{
    [System.Serializable]
    public struct Input
    {
        [Tooltip("Simply a name to easily identify the struct.")]
        public string name;
        [Tooltip("The key inputs that will trigger the event specified.")]
        public KeyCode[] keyInputs;
        [Tooltip("Called when a key from keyInputs is pressed")]
        public UnityEvent onInputTrigger;
    }

    public Input[] inputs;

    void Update()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            Input current = inputs[i];
            if (CheckInputArray(current.keyInputs))
            {
                current.onInputTrigger?.Invoke();
            }
        }
    }

    bool CheckInputArray(KeyCode[] keys)
    {
        for (int i = 0; i < keys.Length; i++)
        {
            if (UnityEngine.Input.GetKeyDown(keys[i]))
            {
                return true;
            }
        }

        return false;
    }
}

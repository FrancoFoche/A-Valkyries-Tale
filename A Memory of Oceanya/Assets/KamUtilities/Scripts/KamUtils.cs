using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kam.CustomInput
{
    public class Utilities_Input_HoldKey
    {
        float curHold;

        bool everyFrame;

        public Utilities_Input_HoldKey(bool everyFrame)
        {
            this.everyFrame = everyFrame;
        }

        /// <summary>
        /// Returns true after the key has been held down for the specified amount of seconds.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool HoldKey(KeyCode key, float seconds)
        {
            if (UnityEngine.Input.GetKey(key))
            {
                curHold += Time.deltaTime;

                if (curHold > seconds)
                {
                    if (!everyFrame)
                    {
                        curHold = 0;
                    }
                    return true;
                }
            }

            if (UnityEngine.Input.GetKeyUp(key))
            {
                curHold = 0;
            }

            return false;
        }
    }
}

namespace Kam.Utils
{
    public class KamUtilities
    {
        public static float Map(float original, float originalMin, float originalMax, float newMin, float newMax)
        {
            return newMin + (original - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }
        
        public static List<T> ForAllNearby<T>(GameObject self, List<T> listOfObjects, float maxDistance, Action<T> action) where T : MonoBehaviour
        {
            List<T> nearby = new List<T>();

            foreach (var obj in listOfObjects)
            {
                if (obj.gameObject != self && Vector3.Distance(obj.transform.position, self.transform.position) < maxDistance)
                {
                    action(obj);
                    nearby.Add(obj);
                }
            }

            return nearby;
        }
    }

    public class KamColor
    {
        public static Color purple = new Color(0.73f, 0.02f, 1f);
    }
}
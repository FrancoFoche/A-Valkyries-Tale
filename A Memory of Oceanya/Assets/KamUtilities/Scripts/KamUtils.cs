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
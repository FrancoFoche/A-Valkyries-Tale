using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<T>
{
    T GetLoaded();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjectInfo
{
    public string name;
    public int id;
    public string description;

    public BaseObjectInfo(string name, int id)
    {
        this.name = name;
        this.id = id;
    }

    public BaseObjectInfo(string name, int id, string description)
    {
        this.name = name;
        this.id = id;
        this.description = description;
    }
}

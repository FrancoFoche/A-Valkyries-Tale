using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;


/// <summary>
/// Inverts the normal Image comparing function so masks are inverted.
/// </summary>
public class Utilities_Mask_InverseCutout : Image
{
    public override Material materialForRendering {
        get 
        {
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material; 
        } 
    }
}

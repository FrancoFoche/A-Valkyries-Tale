using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    public MeshRenderer targetRenderer;
    public Material originalMaterial;

    public void SwitchSprites(Texture2D sprite)
    {
        Material newMaterial = new Material(originalMaterial);
        newMaterial.mainTexture = sprite;
        targetRenderer.material = newMaterial;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Crop")]
public class CropAssets : ScriptableObject
{
    public Sprite seedSprite;
    public Sprite growingSprite;
    public Sprite doneSprite;
}

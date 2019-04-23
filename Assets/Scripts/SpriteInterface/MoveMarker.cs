using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMarker : MonoBehaviour
{
    public Sprite markerSprite;
    public Vector3 markerOffset = new Vector3(0, 0, 0);
    private SpriteRenderer markerImage;
    private Animation anim;

    void Start()
    {
        anim = this.gameObject.GetComponentInChildren<Animation>();
        markerImage = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        markerImage.sprite = markerSprite;
        Disappear();
    }

    public void SetMarker(Tile targetTile)
    {
        Vector3 targetPos = targetTile.pos + markerOffset;
        this.transform.position = targetPos;
        markerImage.color = Color.white;
        anim.Play();
    }

    public void Disappear()
    {
        markerImage.color = Color.clear;
        anim.Stop();
    }
}

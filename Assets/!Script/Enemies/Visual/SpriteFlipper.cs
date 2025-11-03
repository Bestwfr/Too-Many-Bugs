using System;
using FlamingOrange.Enemies;
using PurrNet;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipper : NetworkBehaviour
{
    public GameObject Target;

    private SpriteRenderer spriteRenderer;

    protected override void OnSpawned()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Target == null) return;
        
        if (Target.transform.position.x > transform.position.x)
            spriteRenderer.flipX = true; // Face right
        else if (Target.transform.position.x < transform.position.x)
            spriteRenderer.flipX = false;  // Face left
    }
}

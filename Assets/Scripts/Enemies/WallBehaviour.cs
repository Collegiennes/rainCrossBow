using System;
using UnityEngine;

class WallBehaviour : Enemy
{
    public override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Level.ScrollingSpeed;

        AfterUpdate();
    }
}


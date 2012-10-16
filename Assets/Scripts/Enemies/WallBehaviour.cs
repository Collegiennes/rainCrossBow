using System;
using UnityEngine;

class WallBehaviour : Enemy
{
    const float Speed = 1.125f;

    public override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Speed;

        AfterUpdate();
    }
}


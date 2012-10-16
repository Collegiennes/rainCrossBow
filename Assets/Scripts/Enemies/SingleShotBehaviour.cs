using System;
using UnityEngine;

class SingleShotBehaviour : Enemy
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

        // Doesn't do crap
        var heading = Vector3.Normalize(PlayerTransform.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(heading), 0.25f);

        AfterUpdate();
    }
}


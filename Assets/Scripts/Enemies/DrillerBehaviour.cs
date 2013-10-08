using System;
using UnityEngine;

class DrillerBehaviour : Enemy
{
    const float StrafeSpeed = 1 / 5f;
    const float MoveSpeed = 1 / 4f;

    Vector3 Velocity;

    public override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        var xDistance = Math.Abs(PlayerTransform.position.x - transform.position.x);

        var strafeFactor = 1f;
        if (xDistance < 1)
            strafeFactor = xDistance;

        xDistance /= 2;
        if (xDistance < 1) xDistance = 1;
        //xDistance = (float)Math.Pow(xDistance, 1 / 2f);

        Velocity += -Vector3.up * Time.deltaTime / xDistance * MoveSpeed * Level.ScrollingSpeed;
        if (PlayerTransform.position.y < transform.position.y)
            Velocity += Vector3.right * Math.Sign(PlayerTransform.position.x - transform.position.x) * Time.deltaTime * StrafeSpeed * Level.ScrollingSpeed * strafeFactor;
        Velocity *= 0.9425f;

        transform.position += Velocity;

        AfterUpdate();
    }
}


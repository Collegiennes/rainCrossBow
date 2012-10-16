using System;
using UnityEngine;

class CarrierBehaviour : Enemy
{
    const float Speed = 1.125f;

    public override void OnDie()
    {
        base.OnDie();

        Vector3[] vectors = { Vector3.up, new Vector3(-0.7f, -0.7f, 0), new Vector3(0.7f, -0.7f, 0) };
        int i = 0;
        foreach (var hs in GetComponents<HurterSpawner>())
            hs.ForceSpawn(vectors[i++]);

        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Speed;

        AfterUpdate();
    }
}


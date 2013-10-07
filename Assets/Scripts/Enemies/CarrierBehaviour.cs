using System;
using UnityEngine;

class CarrierBehaviour : Enemy
{
    const float Speed = 1.125f;

    Vector3 center;
    float WiggleStep;
    public float WiggleSpeed = 1.0f;
    public int WiggleSign = 1;
    public float WiggleOffset = 0;

    protected override void Start()
    {
        base.Start();
        center = transform.position;
        WiggleStep = WiggleOffset;
        WiggleSpeed /= 2.0f;
    }

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

        center -= Vector3.up * Time.deltaTime * Speed;

        WiggleStep += WiggleSpeed * Time.deltaTime * WiggleSign;
        transform.position = center + new Vector3(0.5f, 0, 0) * Mathf.Sin(WiggleStep * Mathf.PI);

        AfterUpdate();
    }
}


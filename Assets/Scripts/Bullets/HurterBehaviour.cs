using System;
using UnityEngine;

class HurterBehaviour : Bullet
{
    [HideInInspector]
    public Vector3 HomingAim;
    [HideInInspector]
    public Func<float, float, float> Acceleration;
    [HideInInspector]
    public float MinSpeed;
    [HideInInspector]
    public float MaxSpeed;

    public float Velocity;
    public bool NoInertia;
    public float SpawnTime;

    float SinceAlive;

    public override void Start()
    {
        base.Start();
        SinceAlive = 0;
    }

    void FixedUpdate()
    {
        if (Dead) return;

        var acc = Acceleration(SinceAlive, SpawnTime);
        Velocity += acc * Mathf.Pow(Level.ScrollingSpeed, 2);
        var newVelocity = Velocity * Level.ScrollingSpeed;
        newVelocity = Mathf.Clamp(newVelocity, MinSpeed * Level.ScrollingSpeed, MaxSpeed * Level.ScrollingSpeed);

        transform.position += HomingAim * newVelocity * Level.ScrollingSpeed;
        if (!NoInertia)
            transform.position += Level.ScrollingSpeed * Vector3.down * Time.deltaTime;

        SinceAlive += Time.deltaTime;

        AfterUpdate();
    }
}

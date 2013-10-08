using System;
using UnityEngine;

class HurterBehaviour : Bullet
{
    [HideInInspector]
    public Vector3 HomingAim;
    [HideInInspector]
    public Func<float, float> Acceleration;
    [HideInInspector]
    public float MinSpeed;
    [HideInInspector]
    public float MaxSpeed;

    public float Velocity;
    public bool NoInertia;

    float SinceAlive;

    public override void Start()
    {
        base.Start();
        SinceAlive = 0;
    }

    void FixedUpdate()
    {
        if (Dead) return;

        Velocity += Acceleration(SinceAlive);
        Velocity = Mathf.Clamp(Velocity, MinSpeed, MaxSpeed);

        transform.position += HomingAim * Velocity;
        if (!NoInertia)
            transform.position += Level.ScrollingSpeed * Vector3.down * Time.deltaTime;

        SinceAlive += Time.deltaTime;

        AfterUpdate();
    }
}

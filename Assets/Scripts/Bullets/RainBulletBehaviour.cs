using System;
using System.Collections;
using UnityEngine;

class RainBulletBehaviour : Bullet
{
    const float Speed = 30;

    static readonly float[] Power = { 1.75f, 1.8f, 2.25f };

    [HideInInspector]
    public Vector3 Direction = Vector3.up;

    public override void Start()
    {
        base.Start();

        if (PlayerTransform.gameObject.GetComponent<Shooting>().RainLevel == 3)
            gameObject.FindChild("Sphere.2").renderer.material = ColorShifting.Materials["clr_Shift"];
    }

    protected override void OnHit(Shooting shooting, Enemy enemy, Vector3 contactPoint)
    {
        if (enemy.Health <= 0)
            return;

        var level = shooting.RainLevel;
        enemy.Health -= Power[Math.Min(level - 1, 2)];

        if (level < 3)
            Destroy(gameObject);

        base.OnHit(shooting, enemy, contactPoint);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        BeforeUpdate();

        transform.position += Time.deltaTime * Speed * Direction;

        AfterUpdate();
    }
}

using System.Linq;
using UnityEngine;

class ClockBehaviour : Enemy
{
    const float Speed = 1.125f;
    const float RotationSpeed = 0.01f;

    public bool Clockwise;

    HurterSpawner Spawner;
    Animation[] Animations;

    public override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    void Start()
    {
        Spawner = GetComponent<HurterSpawner>();
        Animations = GetComponentsInChildren<Animation>();

        if (!Clockwise)
            foreach (var a in Animations)
                foreach (AnimationState state in a)
                    state.speed = -state.speed;
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Speed;
        Spawner.ShootingVector = Animations[0].transform.rotation * Vector3.up;

        AfterUpdate();
    }
}

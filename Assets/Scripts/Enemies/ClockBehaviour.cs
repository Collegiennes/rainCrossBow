using System.Linq;
using UnityEngine;

class ClockBehaviour : Enemy
{
    public bool Clockwise;
    public float RotateSpeed;

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

        foreach (var a in Animations)
            foreach (AnimationState state in a)
                state.speed = state.speed * (Clockwise ? RotateSpeed : -RotateSpeed);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Level.ScrollingSpeed;
        Spawner.ShootingVector = Animations[0].transform.rotation * Vector3.up;

        AfterUpdate();
    }
}

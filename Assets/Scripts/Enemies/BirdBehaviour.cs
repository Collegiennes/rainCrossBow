using UnityEngine;

class BirdBehaviour : Enemy
{
    const float Speed = 5;

    public override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position += Vector3.up * Time.deltaTime * Speed;
        AfterUpdate();
    }
}

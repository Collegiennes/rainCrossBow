using UnityEngine;

class PowerUpBehaviour : Enemy
{
    public override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Level.ScrollingSpeed;
        AfterUpdate();
    }
}


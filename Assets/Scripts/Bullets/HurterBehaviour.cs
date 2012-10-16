using UnityEngine;

class HurterBehaviour : Bullet
{
    const float ConstantSpeed = 0.08f;

    [HideInInspector]
    public Vector3 HomingAim = Vector3.down;

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position += HomingAim * ConstantSpeed;

        AfterUpdate();
    }
}

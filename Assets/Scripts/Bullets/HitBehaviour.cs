using UnityEngine;

class HitBehaviour : MonoBehaviour
{
    const float Speed = 10;

    Animation Anim;

    public Vector3 Velocity = Vector3.up;

    void Start()
    {
        Anim = GetComponentInChildren<Animation>();
    }

    public void SetSpeed(float speed)
    {
        Anim = GetComponentInChildren<Animation>();
        foreach (AnimationState state in Anim)
            state.speed = speed;
    }

    void FixedUpdate()
    {
        Velocity *= 0.9f;
        transform.position += Velocity * Speed * Time.deltaTime;

        if (!Anim.isPlaying)
            Destroy(gameObject);
    }
}

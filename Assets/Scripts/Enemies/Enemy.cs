using UnityEngine;

abstract class Enemy : MonoBehaviour
{
    public float Health;

    [HideInInspector] 
    public bool Dead;
    [HideInInspector]
    public bool LockedOn;

    protected Transform PlayerTransform;

    protected virtual void Start()
    {
        PlayerTransform = GameObject.Find("Player").transform;
    }

    public virtual void OnDie()
    {
        Dead = true;
    }

    protected void AfterUpdate()
    {
        // Out-of-screen detection
        if (transform.position.y <= -10)
        {
            Destroy(gameObject);
            return;
        }
    }
}

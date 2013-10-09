using System.Collections.Generic;
using System.Linq;
using UnityEngine;

abstract class Bullet : MonoBehaviour
{
    const float ScreenSize = 11;

    protected readonly HashSet<GameObject> AlreadyHit = new HashSet<GameObject>();
    protected Vector3 LastPosition;

    [HideInInspector]
    public bool Dead;

    protected Transform PlayerTransform;

    public virtual void Start()
    {
        PlayerTransform = GameObject.Find("Player").transform;
    }

    protected virtual void OnHit(Shooting shooting, Enemy enemy, Vector3 contactPoint)
    {
        var dead = enemy.Health <= 0;

        var hitGo = (GameObject)Instantiate(Shooting.HitTemplate);
        hitGo.transform.position = contactPoint + (dead ? Vector3.back * 2 : Vector3.back);
        if (!dead)
            hitGo.transform.localScale /= 2;

        hitGo.GetComponent<HitBehaviour>().Velocity = Vector3.up;
        hitGo.GetComponentInChildren<Renderer>().material = dead ? ColorShifting.Materials["clr_GreyDark"] : ColorShifting.Materials["clr_GreyPale"];

        var randomAmount = 3;
        if (this is CrossBulletBehaviour) randomAmount = 6;

        for (int i = 0; i < randomAmount; i++)
            Wait.Until(t => t >= Random.value, () =>
            {
                var rv = Random.value - 0.5f;
                if (dead) rv /= 2;
                hitGo = (GameObject)Instantiate(Shooting.HitTemplate);
                hitGo.transform.position = contactPoint + (dead ? Vector3.back * 2 : Vector3.back) + new Vector3(rv, rv, 0);
                rv = Random.value - 0.5f;
                hitGo.transform.localScale += new Vector3(rv, rv, rv) * 0.25f;
                hitGo.GetComponent<HitBehaviour>().Velocity = Vector3.up;
                if (!dead)
                    hitGo.transform.localScale /= 2;
                hitGo.GetComponentInChildren<Renderer>().material = dead ? ColorShifting.Materials["clr_GreyDark"] : ColorShifting.Materials["clr_GreyPale"];
            });
    }

    protected void BeforeUpdate()
    {
        LastPosition = transform.position;
    }

    protected virtual void DetectCollisions()
    {
        if (this is HurterBehaviour) return;

        // Enemy collision detection
        var delta = transform.position - LastPosition;
        var distance = delta.magnitude;
        if (!Mathf.Approximately(distance, 0))
        {
            RaycastHit[] hitInfos;
            hitInfos = Physics.RaycastAll(LastPosition, delta / distance, distance, 1 << 8);
            if (hitInfos.Length > 0)
            {
                RaycastHit chosenHit;
                if (hitInfos.Length > 1 && hitInfos.Any(x => x.collider.gameObject.GetComponent<WallBehaviour>() != null))
                {
                    //Debug.Log("Wall had precedence");
                    chosenHit = hitInfos.First(x => x.collider.gameObject.GetComponent<WallBehaviour>() != null);
                }
                else
                    chosenHit = hitInfos[0];

                // test if boxed in, reject if so
                if (!IsBoxedIn(chosenHit.collider))
                    OnCollide(chosenHit.collider, chosenHit.point);
            }
            else
            {
                var colliders = Physics.OverlapSphere(transform.position, 0.25f, 1 << 8);
                Collider chosenCollider;
                if (colliders.Length != 0)
                {
                    if (colliders.Length > 1 && colliders.Any(x => x.gameObject.GetComponent<WallBehaviour>() != null))
                    {
                        //Debug.Log("Wall had precedence");
                        chosenCollider = colliders.First(x => x.gameObject.GetComponent<WallBehaviour>() != null);
                    }
                    else
                        chosenCollider = colliders[0];

                    if (!IsBoxedIn(chosenCollider))
                        OnCollide(chosenCollider, transform.position);
                }
            }
        }
    }

    bool IsBoxedIn(Collider c)
    {
        if (c.gameObject.name.StartsWith("nmy_every")) return false;

        return
            Physics.OverlapSphere(c.transform.position + Vector3.up, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0 &&
            Physics.OverlapSphere(c.transform.position + Vector3.down, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0 &&
            Physics.OverlapSphere(c.transform.position + Vector3.left, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0 &&
            Physics.OverlapSphere(c.transform.position + Vector3.right, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0;
    }

    protected void AfterUpdate()
    {
        DetectCollisions();

        // Out-of-screen detection
        var halfWidth = ScreenSize * Camera.main.aspect;
        const float halfHeight = ScreenSize;

        if (transform.position.x > halfWidth || transform.position.x < -halfWidth ||
            transform.position.y > halfHeight || transform.position.y < -halfHeight)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected void OnCollide(Collider other, Vector3 contactPoint)
    {
        if (AlreadyHit.Contains(other.gameObject)) return;
        AlreadyHit.Add(other.gameObject);

        var otherGO = other.gameObject;
        if (otherGO.tag == "Enemy") // Kind of useless but whatevs'
        {
            var shooting = GameObject.Find("Player").GetComponent<Shooting>();
            var enemy = otherGO.GetComponent<Enemy>();

            OnHit(shooting, enemy, contactPoint);

            if (enemy.Health <= 0) 
                enemy.OnDie();
        }
    }
}

using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

class CrossBulletBehaviour : Bullet
{
    const float TerminalSpeed = 25;

    static readonly float[] Power = { 6.0f, 5.5f, 6.0f };

    Vector3 origin;
    float SinceAlive;
    Enemy LockedTo;
    float SinceVeryClose;

    [HideInInspector]
    public Vector3 Direction = Vector3.up;

    public override void Start()
    {
        base.Start();
        TryLock();

        if (PlayerTransform.gameObject.GetComponent<Shooting>().CrossLevel == 3)
            gameObject.FindChild("Sphere.2").renderer.material = ColorShifting.Materials["clr_Shift"];

        origin = transform.position;
    }

    void TryLock()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(x => x.GetComponent<Enemy>());
        SinceVeryClose = 0;

        var enemy = enemies.Where(e => !e.Dead)
            .Where(e => !(
                Physics.OverlapSphere(e.transform.position + Vector3.up, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0 &&
                Physics.OverlapSphere(e.transform.position + Vector3.down, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0 &&
                Physics.OverlapSphere(e.transform.position + Vector3.left, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0 &&
                Physics.OverlapSphere(e.transform.position + Vector3.right, 0.25f, 1 << LayerMask.NameToLayer("Enemies")).Length != 0)) // reject if boxed in
            .OrderBy(e => (e.LockedOn).AsNumeric())
            .ThenBy(e => Vector3.Distance(e.transform.position, PlayerTransform.position))
            .ThenBy(e => Math.Sign(e.transform.position.y)).FirstOrDefault();

        if (enemy != null)
        {
            //Debug.Log("Locked on to " + enemy.name);

            LockedTo = enemy;
            LockedTo.LockedOn = true;
        }
    }

    protected override void OnHit(Shooting shooting, Enemy enemy, Vector3 contactPoint)
    {
        var level = shooting.CrossLevel;
        enemy.Health -= Power[Math.Min(level - 1, 2)];

        if (LockedTo != null)
            LockedTo.LockedOn = false;

        var dead = enemy.Health <= 0;

        if (dead && !enemy.Dead)
        {
            var xploGo = (GameObject)Instantiate(Shooting.ExplosionTemplate);
            xploGo.transform.position = enemy.transform.position;
            xploGo.transform.rotation = Random.rotation;
            xploGo.renderer.material = ColorShifting.Materials[ColorShifting.EnemyMaterials[enemy.GetType()]];
        }

        base.OnHit(shooting, enemy, contactPoint);

        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        BeforeUpdate();

        SinceAlive += Time.deltaTime;

        if (LockedTo == null || LockedTo.Dead)
            TryLock();

        if (LockedTo != null) // Still no-one?
        {
            var heading = Vector3.Normalize(LockedTo.transform.position - transform.position);
            float headingSpeed;
            if (SinceAlive < 1.25f)
                headingSpeed = Easing.EaseIn(SinceAlive / 1.25f, EasingType.Quadratic) * 0.225f;
            else
                headingSpeed = 0.225f + 0.1f * Mathf.Clamp01(SinceAlive - 1.25f);

            Direction = Vector3.Slerp(Direction, heading, headingSpeed);
            transform.rotation = Quaternion.LookRotation(Direction);

            // test for infinite loop around dude
            if (Vector3.Distance(transform.position, LockedTo.transform.position) < 2.0f)
            {
                SinceVeryClose += Time.deltaTime;
                if (SinceVeryClose > 1.5f)
                {
                    // fake force collide
                    OnCollide(LockedTo.collider, transform.position);
                    return;
                }
            }
            else
                SinceVeryClose = 0;
        }

        var speed = Mathf.Lerp(5, TerminalSpeed, Easing.EaseIn(Mathf.Clamp01(SinceAlive * 1.25f), EasingType.Quintic));
        transform.position += Time.deltaTime * speed * Direction;

        // Trail!
        SpawnAt(transform.position);
        SpawnAt((transform.position + LastPosition) / 2f);

        AfterUpdate();
    }

    void SpawnAt(Vector3 position)
    {
        var rv = (Random.value - 0.5f) * 0.25f;
        var hitGo = (GameObject)Instantiate(Shooting.HitTemplate);
        hitGo.transform.position = position + Vector3.forward + new Vector3(rv, rv, 0);
        rv = Random.value - 0.5f;
        hitGo.transform.localScale += new Vector3(rv, rv, rv) * 0.25f;
        hitGo.transform.localScale /= 2;
        var hb = hitGo.GetComponent<HitBehaviour>();
        hb.Velocity = Vector3.zero;
        hb.SetSpeed(3);
        //hitGo.GetComponentInChildren<Renderer>().material = ColorShifting.Materials["clr_GreyPale"];
    }
}

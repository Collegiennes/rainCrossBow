using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class BowBulletBehaviour : Bullet
{
    const float Speed = 1;

    static readonly float[] Power = { 200, 300, 500 };

    public bool PlayerShot;
    public GameObject CharginTemplate;

    Shooting Shooting;

    float SinceAlive;
    Vector3 Origin;
    float PowerScale;

    public override void Start()
    {
        base.Start();

        Shooting = PlayerTransform.gameObject.GetComponent<Shooting>();

        PowerScale = (float) Math.Pow(PlayerTransform.gameObject.GetComponent<Shooting>().BowLevel, 1);
        if (Shooting.BowLevel == 3)
            gameObject.FindChild("Cube").renderer.material = ColorShifting.Materials["clr_Shift"];
        
        Origin = transform.position + (PlayerShot ? Vector3.up * (PlayerTransform.localScale.y + 0.5f) : Vector3.zero);
        transform.position = Origin + (transform.rotation * Vector3.right) * transform.localScale.x;
        transform.localScale = new Vector3(0, PowerScale, PowerScale);

        if (!PlayerShot)
            SinceAlive = -0.25f;
    }

    protected override void OnHit(Shooting shooting, Enemy enemy, Vector3 contactPoint)
    {
        var level = shooting.BowLevel;
        enemy.Health -= Power[Math.Min(level - 1, 2)] * Time.deltaTime;

        base.OnHit(shooting, enemy, contactPoint);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        Origin = PlayerTransform.position + (PlayerShot ? Vector3.up * (PlayerTransform.localScale.y + 0.5f) : Vector3.zero);

        if (SinceAlive > 0)
        {
            transform.localScale += new Vector3(Speed, 0, 0);
            transform.position = Origin + (transform.rotation * Vector3.right) * transform.localScale.x;
            DetectCollisions();
        }

        SinceAlive += Time.deltaTime;
        if (SinceAlive >= 0.5f * ((Shooting.BowLevel - 1) / 2f + 1))
            Destroy(gameObject);
    }

    protected override void DetectCollisions()
    {
        // Enemy collision detection
        RaycastHit hitInfo;
        var ray = new Ray(Origin, transform.rotation * Vector3.right);
        var size = transform.localScale.x;
        if (transform.localScale.x <= 0.1f)
            size = 0.1f;
        if (Physics.Raycast(ray, out hitInfo, size * 2, 1 << 8))
        {
            if (hitInfo.collider.tag != "Enemy") return;

            OnCollide(hitInfo.collider, hitInfo.point);
            AlreadyHit.Remove(hitInfo.collider.gameObject);

            GetComponentInChildren<Stroboscope>().enabled = true;

            var collidee = hitInfo.collider.transform;
            var lastCollidePosition = collidee.position;

            var cp = ClosestPointOnRay(ray, lastCollidePosition - new Vector3(0, 0.4f, 0));
            var enemy = hitInfo.collider.GetComponent<Enemy>();
            var mat = ColorShifting.Materials[ColorShifting.EnemyMaterials[enemy.GetType()]];
            if (enemy is SingleShotBehaviour && (enemy as SingleShotBehaviour).invertColors)
                mat = ColorShifting.Materials["clr_Red"];

            var go = (GameObject)Instantiate(CharginTemplate, cp, Quaternion.identity);
            go.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            go.FindChild("Sphere.2").renderer.material = mat;
            go.AddComponent<FlickerOut>();

            var hitPoint = collidee == null ? lastCollidePosition : collidee.position;

            var distance = Vector3.Distance(hitPoint, Origin);
            transform.localScale = VectorEx.Modulate(transform.localScale, new Vector3(0, 1, 1)) + (distance - 0.5f) / 2 * Vector3.right;
            transform.position = Origin + (transform.rotation * Vector3.right) * transform.localScale.x;
        }
    }

    static Vector3 ClosestPointOnRay(Ray ray, Vector3 point)
    {
        var w = point - ray.origin;
        var vsq = Vector3.Dot(ray.direction, ray.direction);
        var proj = Vector3.Dot(w, ray.direction);
        return ray.origin + (proj / vsq) * ray.direction;
    }
}


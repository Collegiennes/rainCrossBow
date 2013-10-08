using System;
using UnityEngine;

class SingleShotBehaviour : Enemy
{
    HurterSpawner hs;
    public bool invertColors = false;

    protected override void Start()
    {
        base.Start();
        hs = GetComponent<HurterSpawner>();

        hs.BaseAngle -= 90;

        transform.localRotation = Quaternion.AngleAxis(hs.BaseAngle, Vector3.forward);
        hs.BaseAngle = 360 - hs.BaseAngle;
        hs.ShootingVector = new Vector3(Mathf.Sin(hs.BaseAngle * Mathf.Deg2Rad), Mathf.Cos(hs.BaseAngle * Mathf.Deg2Rad), 0);

        if (invertColors)
        {
            foreach (var r in GetComponentsInChildren<Renderer>())
            {
                //Debug.Log(r.material.name);

                if (r.material.name.StartsWith("clr_Red"))
                    r.material.SetColor("_Emission", new Color(84 / 255.0f, 188 / 255.0f, 28 / 255.0f));
                else if (r.material.name.StartsWith("clr_Green"))
                    r.material.SetColor("_Emission", new Color(255 / 255.0f, 17 / 255.0f, 17 / 255.0f));
            }
        }
    }

    public override void OnDie()
    {
        base.OnDie();
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Level.ScrollingSpeed;

        if (hs.Homing)
        {
            var heading = Vector3.Normalize(PlayerTransform.transform.position - transform.position);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.AngleAxis(-90 + Mathf.Rad2Deg * Mathf.Atan2(heading.y, heading.x), Vector3.forward), 0.25f);
        }

        AfterUpdate();
    }
}


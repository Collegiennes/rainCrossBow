using System;
using UnityEngine;

class HurterSpawner : MonoBehaviour
{
    public GameObject HurterTemplate;
    public bool Homing;
    public float ShootEverySeconds;
    public int ShootOffset;
    public int ShootPauseTime;
    public Func<float, float> Acceleration;
    public float BaseAngle;

    public float SinceAlive;
    int SinceShot;

    public Vector3 ShootingVector;
    public Material CustomMaterial;

    Transform PlayerTransform;
    Enemy Enemy;

    void Start()
    {
        PlayerTransform = GameObject.Find("Player").transform;
        Enemy = GetComponent<Enemy>();
        SinceShot = ShootPauseTime - ShootOffset;
    }

    void FixedUpdate()
    {
        if (Enemy.Dead) return;

        SinceAlive += Time.deltaTime;

        if (SinceAlive >= ShootEverySeconds)
        {
            if (SinceShot >= ShootPauseTime)
            {
                ForceSpawn(null);
                SinceShot = 0;
            }
            else
                SinceShot++;

            SinceAlive -= ShootEverySeconds;
        }
    }

    public void ForceSpawn(Vector3? forcedVector)
    {
        var go = (GameObject)Instantiate(HurterTemplate, transform.position, Quaternion.identity);
        var hb = go.GetComponent<HurterBehaviour>();
        hb.Acceleration = Acceleration;

        if (!Homing)
            hb.HomingAim = forcedVector ?? ShootingVector;
        else
            hb.HomingAim = Vector3.Normalize(PlayerTransform.transform.position - transform.position);

        hb.NoInertia = Homing;
        hb.Velocity = 0.04f;
        hb.MinSpeed = 0.01f;
        hb.MaxSpeed = 0.25f;

        if (CustomMaterial == default(Material))
            go.FindChild("Sphere").renderer.material = ColorShifting.Materials[ColorShifting.EnemyMaterials[gameObject.GetComponent<Enemy>().GetType()]];
        else
            go.FindChild("Sphere").renderer.material = CustomMaterial;
    }
}

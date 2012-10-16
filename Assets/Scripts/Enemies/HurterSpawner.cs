using System;
using UnityEngine;

class HurterSpawner : MonoBehaviour
{
    public GameObject HurterTemplate;
    public bool Homing;
    public float ShootEverySeconds;
    public int ShootPauseRatio;

    float SinceAlive;
    int RatioKeeper = 1;

    public Vector3 ShootingVector;
    public Material CustomMaterial;

    Transform PlayerTransform;
    Enemy Enemy;

    void Start()
    {
        PlayerTransform = GameObject.Find("Player").transform;
        Enemy = GetComponent<Enemy>();
    }

    void FixedUpdate()
    {
        if (Enemy.Dead) return;

        SinceAlive += Time.deltaTime;

        if (SinceAlive >= ShootEverySeconds * 1.125f)
        {
            if (RatioKeeper <= ShootPauseRatio)
            {
                ForceSpawn(null);
            }

            RatioKeeper++;
            if (RatioKeeper > 5) RatioKeeper = 1;

            SinceAlive -= ShootEverySeconds * 1.125f;
        }
    }

    public void ForceSpawn(Vector3? forcedVector)
    {
        var go = (GameObject)Instantiate(HurterTemplate, transform.position, Quaternion.identity);
        var hb = go.GetComponent<HurterBehaviour>();

        if (!Homing)
            hb.HomingAim = forcedVector ?? ShootingVector;
        else
            hb.HomingAim = Vector3.Normalize(PlayerTransform.transform.position - transform.position);

        if (CustomMaterial == default(Material))
            go.FindChild("Sphere").renderer.material = ColorShifting.Materials[ColorShifting.EnemyMaterials[gameObject.GetComponent<Enemy>().GetType()]];
        else
            go.FindChild("Sphere").renderer.material = CustomMaterial;
    }
}

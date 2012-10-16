using System;
using UnityEngine;

class EverynianBehaviour : Enemy
{
    const float Speed = 1.125f;

    public GameObject BirdTemplate;

    GameObject ToSpawn;

    void Start()
    {
        var holder = gameObject.FindChild("powerup_holder");

        var bow = holder.FindChild("pwr_bow");
        var cross = holder.FindChild("pwr_cross");
        var rain = holder.FindChild("pwr_rain");

        bow.SetActiveRecursively(false); cross.SetActiveRecursively(false); rain.SetActiveRecursively(false);

        var enabledId = RandomHelper.Random.Next(0, 3);
        if (enabledId == 0) { bow.SetActiveRecursively(true); ToSpawn = bow; }
        if (enabledId == 1) { cross.SetActiveRecursively(true); ToSpawn = cross; }
        if (enabledId == 2) { rain.SetActiveRecursively(true); ToSpawn = rain; }
    }

    public override void OnDie()
    {
        base.OnDie();

        var go = (GameObject)Instantiate(ToSpawn, transform.position, Quaternion.Euler(0, 90, 0));
        go.transform.localScale = VectorEx.Modulate(new Vector3(0.5967718f, 0.5967718f, 0.5967718f),
                                                    go.transform.localScale);
        go.GetComponent<PowerUpBehaviour>().enabled = true;

        go = (GameObject)Instantiate(BirdTemplate);
        go.transform.position = transform.position;

        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Speed;

        AfterUpdate();
    }
}


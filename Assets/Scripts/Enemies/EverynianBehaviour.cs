using System;
using System.Collections.Generic;
using UnityEngine;

class EverynianBehaviour : Enemy
{
    public GameObject BirdTemplate;

    GameObject ToSpawn;
    public int? ForcedId;

    static int? LastGiven = null;

    void Start()
    {
        var holder = gameObject.FindChild("powerup_holder");

        var bow = holder.FindChild("pwr_bow");
        var cross = holder.FindChild("pwr_cross");
        var rain = holder.FindChild("pwr_rain");

        bow.SetActiveRecursively(false); cross.SetActiveRecursively(false); rain.SetActiveRecursively(false);

        var domain = new List<int>(new [] { 0, 1, 2 });

        var shooting = GameObject.Find("Player").transform.GetComponent<Shooting>();
        if (shooting.BowLevel == 3) domain.Remove(0);
        if (shooting.CrossLevel == 3) domain.Remove(1);
        if (shooting.RainLevel == 3) domain.Remove(2);
        if (LastGiven != null) domain.Remove(LastGiven.Value);
        if (domain.Count == 0) domain.AddRange(new [] { 0, 1, 2 });

        var enabledId = ForcedId.HasValue ? ForcedId.Value : RandomHelper.InEnumerable(domain);

        //var enabledId = 1;
        if (enabledId == 0) { bow.SetActiveRecursively(true); ToSpawn = bow; }
        if (enabledId == 1) { cross.SetActiveRecursively(true); ToSpawn = cross; }
        if (enabledId == 2) { rain.SetActiveRecursively(true); ToSpawn = rain; }

        LastGiven = enabledId;
    }

    public override void OnDie()
    {
        base.OnDie();

        var go = (GameObject)Instantiate(ToSpawn.transform.parent.gameObject);
        go.transform.position = transform.position;
        go.transform.localScale = VectorEx.Modulate(new Vector3(0.5967718f, 0.5967718f, 0.5967718f),
                                                    go.transform.localScale);
        go.FindChild(ToSpawn.name).GetComponent<PowerUpBehaviour>().enabled = true;

        go = (GameObject)Instantiate(BirdTemplate);
        go.transform.position = transform.position;

        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        transform.position -= Vector3.up * Time.deltaTime * Level.ScrollingSpeed;

        AfterUpdate();
    }
}


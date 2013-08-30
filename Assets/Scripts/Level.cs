using System;
using System.Collections.Generic;
using UnityEngine;

class Level : MonoBehaviour
{
    public static GameObject DrillTemplate;
    public static GameObject CarrierTemplate;
    public static GameObject WallTemplate;
    public static GameObject SingleShotTemplate;
    public static GameObject EverynianTemplate;
    public static GameObject BombTemplate;
    public static GameObject ClockTemplate;

    public readonly List<SpawnState> DrillSpawns = new List<SpawnState>();
    public readonly List<HurterSpawnState> CarrierSpawns = new List<HurterSpawnState>();
    public readonly List<HurterSpawnState> SingleShotSpawns = new List<HurterSpawnState>();
    public readonly List<SpawnState> EverynianSpawns = new List<SpawnState>();
    public readonly List<SpawnState> WallSpawns = new List<SpawnState>();
    public readonly List<ClockSpawnState> ClockSpawns = new List<ClockSpawnState>();
    //public readonly List<SpawnState> BombSpawns = new List<SpawnState>();

    public float SinceAlive;

    public void AddDrill(SpawnState spawnState)
    {
        DrillSpawns.Add(spawnState);
    }
    public void AddCarrier(HurterSpawnState spawnState)
    {
        CarrierSpawns.Add(spawnState);
    }
    public void AddWall(SpawnState spawnState)
    {
        WallSpawns.Add(spawnState);
    }
    public void AddSingleShot(HurterSpawnState spawnState)
    {
        SingleShotSpawns.Add(spawnState);
    }
    public void AddClock(ClockSpawnState spawnState)
    {
        ClockSpawns.Add(spawnState);
    }
    public void AddEverynian(SpawnState spawnState)
    {
        EverynianSpawns.Add(spawnState);
    }

    public void Finish()
    {
        DrillSpawns.Sort((a, b) => a.AtTime.CompareTo(b.AtTime));
        CarrierSpawns.Sort((a, b) => a.AtTime.CompareTo(b.AtTime));
        SingleShotSpawns.Sort((a, b) => a.AtTime.CompareTo(b.AtTime));
        EverynianSpawns.Sort((a, b) => a.AtTime.CompareTo(b.AtTime));
        WallSpawns.Sort((a, b) => a.AtTime.CompareTo(b.AtTime));
        ClockSpawns.Sort((a, b) => a.AtTime.CompareTo(b.AtTime));
    }

    readonly HashSet<int> thisSpawnedAt = new HashSet<int>(); 

    void FixedUpdate()
    {
        SinceAlive += Time.deltaTime;

        thisSpawnedAt.Clear();

        var speedBoost = Mathf.Clamp01(SinceAlive / 60.0f);
        var lateSpeedBoost = Mathf.Clamp01((SinceAlive - 30.0f) / 60.0f);

        for (int i = 0; i < DrillSpawns.Count; i++)
        {
            var spawn = DrillSpawns[i];
            if (spawn.AtTime <= SinceAlive)
            {
                Instantiate(DrillTemplate, new Vector3(spawn.Position, 10, 0), Quaternion.Euler(90, 0, 0));
                DrillSpawns.RemoveAt(i--);
            }
            else
                break;
        }

        for (int i = 0; i < CarrierSpawns.Count; i++)
        {
            var spawn = CarrierSpawns[i];
            if (spawn.AtTime <= SinceAlive)
            {
                thisSpawnedAt.Add(spawn.Position);
                var go = (GameObject)Instantiate(CarrierTemplate, new Vector3(spawn.Position, 10, 0), Quaternion.Euler(0, 90, 0));
                CarrierSpawns.RemoveAt(i--);

                foreach (var hs in go.GetComponents<HurterSpawner>())
                {
                    hs.ShootEverySeconds = spawn.ShootEvery * (2.0f - speedBoost * 1.25f);
                    hs.Homing = false;
                    hs.ShootPauseRatio = spawn.ShootRatio;
                }
            }
            else
                break;
        }

        for (int i = 0; i < EverynianSpawns.Count; i++)
        {
            var spawn = EverynianSpawns[i];
            if (spawn.AtTime <= SinceAlive)
            {
                Instantiate(EverynianTemplate, new Vector3(spawn.Position, 10, 0), Quaternion.identity);
                EverynianSpawns.RemoveAt(i--);
            }
            else
                break;
        }

        for (int i = 0; i < WallSpawns.Count; i++)
        {
            var spawn = WallSpawns[i];
            if (spawn.AtTime <= SinceAlive)
            {
                if (thisSpawnedAt.Contains(spawn.Position))
                {
                    spawn.AtTime++;
                    continue;
                }
                thisSpawnedAt.Add(spawn.Position);
                Instantiate(WallTemplate, new Vector3(spawn.Position, 10, 0), Quaternion.Euler(0, 90, 0));
                WallSpawns.RemoveAt(i--);
            }
            else
                break;
        }

        for (int i = 0; i < SingleShotSpawns.Count; i++)
        {
            var spawn = SingleShotSpawns[i];
            if (spawn.AtTime <= SinceAlive)
            {
                if (thisSpawnedAt.Contains(spawn.Position))
                {
                    spawn.AtTime++;
                    continue;
                }
                thisSpawnedAt.Add(spawn.Position);
                var go = (GameObject)Instantiate(SingleShotTemplate, new Vector3(spawn.Position, 10, 0), Quaternion.Euler(0, 90, 0));
                var hs = go.GetComponent<HurterSpawner>();
                hs.ShootEverySeconds = spawn.ShootEvery * (2.0f - lateSpeedBoost * 1.25f);
                hs.Homing = true;
                hs.ShootPauseRatio = spawn.ShootRatio;
                SingleShotSpawns.RemoveAt(i--);
            }
            else
                break;
        }

        for (int i = 0; i < ClockSpawns.Count; i++)
        {
            var spawn = ClockSpawns[i];
            if (spawn.AtTime <= SinceAlive)
            {
                if (thisSpawnedAt.Contains(spawn.Position))
                {
                    spawn.AtTime++;
                    continue;
                }
                thisSpawnedAt.Add(spawn.Position);
                var go = (GameObject)Instantiate(ClockTemplate, new Vector3(spawn.Position, 10, 0), Quaternion.Euler(0, 90, 0));
                var hs = go.GetComponent<HurterSpawner>();
                hs.ShootEverySeconds = spawn.ShootEvery - speedBoost * 0.125f;
                hs.ShootPauseRatio = spawn.ShootRatio;
                hs.Homing = false;
                go.GetComponent<ClockBehaviour>().Clockwise = spawn.Clockwise;
                ClockSpawns.RemoveAt(i--);
            }
            else
                break;
        }
    }
}

public class SpawnState
{
    public int AtTime;
    public int Position;
}

public class HurterSpawnState : SpawnState
{
    public float ShootEvery;
    public int ShootRatio;
}

public class ClockSpawnState : HurterSpawnState
{
    public ClockSpawnState()
    {
        ShootEvery = 0.225f;
    }

    public bool Clockwise;
}

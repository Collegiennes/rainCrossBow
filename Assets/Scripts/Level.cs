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

                if (spawn is WiggleSpawnState)
                {
                    var cb = go.GetComponent<CarrierBehaviour>();
                    var wss = spawn as WiggleSpawnState;
                    cb.WiggleOffset = wss.WiggleDelay;
                    cb.WiggleSign = wss.WiggleSign;
                    cb.WiggleSpeed = wss.WiggleSpeed;
                }

                foreach (var hs in go.GetComponents<HurterSpawner>())
                {
                    hs.ShootEverySeconds = spawn.ShootEvery;
                    hs.Homing = false;
                    hs.ShootPauseTime = spawn.ShootPauseTime;
                    hs.ShootOffset = spawn.ShootPauseOffset;
                    hs.Acceleration = spawn.Acceleration;
                    hs.SinceAlive = spawn.ShootEvery - spawn.TimeDelay;
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
                var go = (GameObject)Instantiate(EverynianTemplate, new Vector3(spawn.Position, 10, 0), Quaternion.identity);
                var es = go.GetComponent<EverynianBehaviour>();

                if (spawn is EverynianState)
                    es.ForcedId = (spawn as EverynianState).ForcedPowerup;

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
                hs.ShootEverySeconds = spawn.ShootEvery;
                hs.ShootPauseTime = spawn.ShootPauseTime;
                hs.ShootOffset = spawn.ShootPauseOffset;
                hs.Acceleration = spawn.Acceleration;
                hs.SinceAlive = spawn.ShootEvery - spawn.TimeDelay;
                if (spawn is SingleShotSpawnState)
                {
                    var ssss = spawn as SingleShotSpawnState;
                    hs.Homing = ssss.Homing;
                    hs.BaseAngle = ssss.Angle - 90;
                    go.GetComponent<SingleShotBehaviour>().invertColors = ssss.Dangerous;
                }
                else
                {
                    hs.BaseAngle = -90;
                }
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
                hs.ShootEverySeconds = spawn.ShootEvery;
                hs.ShootPauseTime = spawn.ShootPauseTime;
                hs.ShootOffset = spawn.ShootPauseOffset;
                hs.Acceleration = spawn.Acceleration;
                hs.SinceAlive = spawn.ShootEvery - spawn.TimeDelay;
                hs.Homing = false;
                go.GetComponent<ClockBehaviour>().Clockwise = spawn.Clockwise;
                go.GetComponent<ClockBehaviour>().RotateSpeed = spawn.RotateSpeed;
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
    public int ShootPauseTime;
    public int ShootPauseOffset;
    public Func<float, float> Acceleration = _ => 0;
    public float TimeDelay = 1;
}

public class ClockSpawnState : HurterSpawnState
{
    public bool Clockwise;
    public float RotateSpeed;
}

public class EverynianState : SpawnState
{
    public int? ForcedPowerup;
}

public class SingleShotSpawnState : HurterSpawnState
{
    public bool Homing;
    public float Angle;
    public bool Dangerous;
}

public class WiggleSpawnState : HurterSpawnState
{
    public float WiggleSpeed = 1;
    public int WiggleSign = 1;
    public float WiggleDelay;
}
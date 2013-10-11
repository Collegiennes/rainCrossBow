using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

class Setup : MonoBehaviour
{
    static GameObject _LevelTemplate;
    public GameObject LevelTemplate;

    public GameObject DrillTemplate;
    public GameObject CarrierTemplate;
    public GameObject WallTemplate;
    public GameObject SingleShotTemplate;
    public GameObject EverynianTemplate;
    public GameObject BombTemplate;
    public GameObject ClockTemplate;

    public GameObject HitTemplate;
    public GameObject ExplosionTemplate;

    public List<Material> Materials = new List<Material>();

    public static Level ActiveLevel;

    void Awake()
    {
        Level.DrillTemplate = DrillTemplate;
        Level.CarrierTemplate = CarrierTemplate;
        Level.WallTemplate = WallTemplate;
        Level.SingleShotTemplate = SingleShotTemplate;
        Level.BombTemplate = BombTemplate;
        Level.ClockTemplate = ClockTemplate;
        Level.EverynianTemplate = EverynianTemplate;

        Shooting.HitTemplate = HitTemplate;
        Shooting.ExplosionTemplate = ExplosionTemplate;

        ColorShifting.Materials = Materials.ToDictionary(x => x.name);

        _LevelTemplate = LevelTemplate;

        Screen.showCursor = false;
    }

    public static void LevelOne()
    {
        Camera.main.audio.Play();

        var levelGO = (GameObject) Instantiate(_LevelTemplate);
        var level = levelGO.GetComponent<Level>();
        ActiveLevel = level;

        var m = new Dictionary<Events, int>();

        // start with a drill wave 'cause it's fun
        AddEvent(level, Events.DrillerWave, 0);
        m.Add(Events.DrillerWave, 0);

        var timeOffset = 0;
        for (int i = 0; i < 150; i++)
        {
            var randomEvent = RandomHelper.InEnum<Events>();
            while (m.ContainsKey(randomEvent) || !randomEvent.ProbabilityRoll())
                randomEvent = RandomHelper.InEnum<Events>();

            timeOffset += AddEvent(level, randomEvent, timeOffset) + GetInterEventGap(i);

            foreach (var k in m.Keys.ToArray())
            {
                m[k] = m[k] + 1;
                if (m[k] > k.GetMinInterspawn())
                    m.Remove(k);
            }

            m.Add(randomEvent, 0);
        }

        level.Finish();
    }

    static int GetInterEventGap(int eventId)
    {
        return 1;
    }

    static int AddEvent(Level level, Events @event, int timeOffset)
    {
        var t = timeOffset;
        bool addEverynyan = RandomHelper.Probability(0.5);

        switch (@event)
        {
            case Events.SoloEverynian:
                level.AddEverynian(new EverynianState { AtTime = t, Position = 0 });
                return 2;

            case Events.BoxedMegaclock:
            {
                var xOffset = RandomHelper.Random.Next(-4, 5);

                level.AddWall(new SpawnState { AtTime = t, Position = xOffset });
                level.AddWall(new SpawnState { AtTime = t + 1, Position = xOffset - 1 });
                level.AddClock(new ClockSpawnState { AtTime = t + 1, Position = xOffset, TimeDelay = 0, ShootEvery = 0.237f, RotateSpeed = 1.0f, Acceleration = (s, st) => -0.0000725f - Mathf.Pow(st, 2.0f) * 0.00000125f });
                level.AddWall(new SpawnState { AtTime = t + 1, Position = xOffset + 1 });
                level.AddWall(new SpawnState { AtTime = t + 2, Position = xOffset });
                return 4;
            }

            case Events.DrillerWave:
                level.AddDrill(new SpawnState { AtTime = t, Position = 0 });
                level.AddDrill(new SpawnState { AtTime = t + 1, Position = -7 });
                level.AddDrill(new SpawnState { AtTime = t + 1, Position = 5 });
                level.AddDrill(new SpawnState { AtTime = t + 2, Position = 7 });
                level.AddDrill(new SpawnState { AtTime = t + 2, Position = -5 });
                level.AddDrill(new SpawnState { AtTime = t + 3, Position = -7 });
                level.AddDrill(new SpawnState { AtTime = t + 3, Position = 5 });
                level.AddDrill(new SpawnState { AtTime = t + 4, Position = 7 });
                level.AddDrill(new SpawnState { AtTime = t + 4, Position = -5 });
                return 0;

            case Events.Corridor:
            {
                var width = RandomHelper.Random.Next(5, 8);
                var length = RandomHelper.Random.Next(4, 8);
                for (int i = 0; i <= length; i++)
                {
                    float step = (i - (length / 2.0f)) / (length / 2.0f);

                    level.AddSingleShot(new SingleShotSpawnState { AtTime = t + i, Position = -width, ShootEvery = 2, Angle = 90 + step * 35, TimeDelay = i / 6f });
                    level.AddSingleShot(new SingleShotSpawnState { AtTime = t + i, Position = width, ShootEvery = 2, Angle = -90 - step * 35, TimeDelay = i / 6f });
                }
                if (addEverynyan)
                    level.AddEverynian(new EverynianState { AtTime = t + length / 3, Position = 0 });
                return length;
            }

//            case Events.EverynyanTriplet:
//                level.AddEverynian(new EverynianState { AtTime = t, Position = -1, ForcedPowerup = 0 });
//                level.AddEverynian(new EverynianState { AtTime = t, Position = -6, ForcedPowerup = 1 });
//                level.AddEverynian(new EverynianState { AtTime = t, Position = 5, ForcedPowerup = 2 });
//                return 3;

            case Events.FullWall:
                for (int i = -8; i < 9; i += 2)
                    level.AddWall(new SpawnState { AtTime = t, Position = i });
                return 2;

            case Events.HomingPyramid:
                level.AddWall(new SpawnState { AtTime = t + 2, Position = 0 });

                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 2, Position = -6, ShootEvery = 0.625f, ShootPauseTime = 2, ShootPauseOffset = 0, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 2, Position = 6, ShootEvery = 0.625f, ShootPauseTime = 2, ShootPauseOffset = 0, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = -3, ShootEvery = 0.625f, ShootPauseTime = 2, ShootPauseOffset = -1, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = 3, ShootEvery = 0.625f, ShootPauseTime = 2, ShootPauseOffset = -1, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 0, Position = 0, ShootEvery = 0.625f, ShootPauseTime = 2, Homing = true });
                return 4;

            case Events.DualTriangles:
            {
                var xOffset = RandomHelper.Random.Next(4, 7);

                var addWalls = RandomHelper.Probability(0.5);

                if (addWalls)
                {
                    level.AddWall(new SpawnState { AtTime = t + 1, Position = -xOffset });
                    level.AddWall(new SpawnState { AtTime = t + 1, Position = xOffset });
                }
                else
                {
                    level.AddWall(new SpawnState { AtTime = t, Position = -1 });
                    //level.AddWall(new SpawnState { AtTime = t + 1, Position = 0 });
                    level.AddWall(new SpawnState { AtTime = t, Position = 1 });
                }

                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = -1 + xOffset, ShootEvery = 1.25f, ShootPauseTime = 1, Angle = 30 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = 1 + xOffset, ShootEvery = 1.25f, ShootPauseTime = 1, Angle = -30 });
                level.AddSingleShot(new HurterSpawnState { AtTime = t, Position = xOffset, ShootEvery = 1.25f, ShootPauseTime = 1, ShootPauseOffset = 1});

                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = -1 - xOffset, ShootEvery = 1.25f, ShootPauseOffset = 1, ShootPauseTime = 1, Angle = 30 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = 1 - xOffset, ShootEvery = 1.25f, ShootPauseOffset = 1, ShootPauseTime = 1, Angle = -30 });
                level.AddSingleShot(new HurterSpawnState { AtTime = t, Position = -xOffset, ShootEvery = 1.25f, ShootPauseTime = 1, ShootPauseOffset = 0 });

                if (addEverynyan)
                    level.AddEverynian(new SpawnState { AtTime = t + 2, Position = 0 });
                return 3 + (addEverynyan ? 2 : 0);
            }

            case Events.MultiRail:
            {
                var count = RandomHelper.Random.Next(2, 4);
                var walled = RandomHelper.Probability(0.5);

                for (int j = 0; j < count; j++)
                {
                    float step = (j - ((count - 1) / 2.0f)) / ((count - 1) / 2.0f);
                    int xOffset = (int)Math.Round(step * (count * 1.5f));
                    level.AddSingleShot(new SingleShotSpawnState { AtTime = t+1, Position = -xOffset, ShootEvery = 3.0f, Acceleration = (s, st) => Mathf.Clamp01(s - 0.75f) * 0.05f, Homing = true, Dangerous = true });
                    
                    if (walled)
                        level.AddWall(new SpawnState { AtTime = t, Position = xOffset });
                }
                return 2;
            }

            case Events.Rail:
            {
                var xOffset = RandomHelper.Random.Next(-6, 7);
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t+1, Position = xOffset, ShootEvery = 3.0f, Angle = 5, Acceleration = (s, st) => Mathf.Clamp01(s - 0.75f) * 0.05f, Homing = true, Dangerous = true });
                if (RandomHelper.Probability(0.5))
                    level.AddWall(new SpawnState { AtTime = t, Position = xOffset });
                return 2;
            }

            case Events.Walled:
                level.AddWall(new SpawnState { AtTime = t, Position = -3 });
                level.AddClock(new ClockSpawnState { AtTime = t + 1, Position = -3, ShootEvery = 0.325f, ShootPauseTime = 1, RotateSpeed = 0.5f, Clockwise = true });
                level.AddWall(new SpawnState { AtTime = t, Position = 3 });
                level.AddClock(new ClockSpawnState { AtTime = t + 1, Position = 3, ShootEvery = 0.325f, ShootPauseTime = 1, ShootPauseOffset = 1, RotateSpeed = 0.5f, Clockwise = false });
                level.AddCarrier(new WiggleSpawnState { AtTime = t + 4, Position = -6, ShootEvery = 3.0f, WiggleSign = -1 });
                level.AddCarrier(new HurterSpawnState { AtTime = t + 4, Position = 6, ShootEvery = 3.0f });
                if (addEverynyan)
                    level.AddEverynian(new SpawnState { AtTime = t + 3, Position = 0 });

                return 5;

            case Events.Wigglers:
            {
                var length = RandomHelper.Random.Next(3, 5);
                for (int i = 0; i < length; i++)
                {
                    var perLine = RandomHelper.Random.Next(2, 5);

                    if (perLine == 2 || perLine == 4)
                        level.AddWall(new SpawnState { AtTime = t + i * 2, Position = 0 });

                    for (int j = 0; j < perLine; j++)
                    {
                        float step = (j - ((perLine-1) / 2.0f)) / ((perLine-1) / 2.0f);
                        int xOffset = (int) Math.Round(step * (perLine * 1.5f));
                        level.AddCarrier(new WiggleSpawnState { AtTime = t + i * 2, Position = xOffset, ShootEvery = 1.5f, ShootPauseTime = perLine, ShootPauseOffset = j, WiggleSpeed = RandomHelper.Between(1, 5), WiggleSign = RandomHelper.Probability(0.5) ? 1 : -1, WiggleDelay = RandomHelper.Between(0, 3) });
                    }
                }

                if (addEverynyan)
                    level.AddEverynian(new SpawnState { AtTime = t + length * 2, Position = 0 });

                return length * 2 + (addEverynyan ? 2 : 0);
            }

            case Events.Focal:
            {
                t++;

                level.AddSingleShot(new SingleShotSpawnState { AtTime = t, Position = -7, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 0, Angle = Mathf.Rad2Deg * Mathf.Atan2(-1, 7) + 90 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 2, Position = -6, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 0, Angle = Mathf.Rad2Deg * Mathf.Atan2(-3, 6) + 90 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 4, Position = -5, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 0, Angle = Mathf.Rad2Deg * Mathf.Atan2(-5, 5) + 90 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 5, Position = -2, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 0, Angle = Mathf.Rad2Deg * Mathf.Atan2(-6, 2) + 90 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 5, Position = 2, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 1, Angle = Mathf.Rad2Deg * Mathf.Atan2(-6, -2) + 90 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 4, Position = 5, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 1, Angle = Mathf.Rad2Deg * Mathf.Atan2(-5, -5) + 90 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 2, Position = 6, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 1, Angle = Mathf.Rad2Deg * Mathf.Atan2(-3, -6) + 90 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t, Position = 7, ShootEvery = 0.6f, ShootPauseTime = 4, ShootPauseOffset = 1, Angle = Mathf.Rad2Deg * Mathf.Atan2(-1, -7) + 90 });

                level.AddWall(new SpawnState { AtTime = t - 1, Position = -7 });
                level.AddWall(new SpawnState { AtTime = t - 1, Position = 7 });

                if (addEverynyan)
                {
                    level.AddEverynian(new SpawnState { AtTime = t + 2, Position = 0 });
                    level.AddWall(new SpawnState { AtTime = t + 1, Position = -1 });
                    //level.AddWall(new SpawnState { AtTime = t + 1, Position = 0 });
                    level.AddWall(new SpawnState { AtTime = t + 1, Position = 1 });
                }
                else
                    level.AddWall(new SpawnState { AtTime = t + 2, Position = 0 });

                return 6;
            }
        }

        return 0;
    }
}

enum Events
{
    HomingPyramid,
    DrillerWave,
    Walled,
    BoxedMegaclock,
    //EverynyanTriplet,
    SoloEverynian,
    Rail,
    Corridor,
    FullWall,
    DualTriangles,
    MultiRail,
    Wigglers,
    Focal
}


static class EventExtensions
{
    public static int GetMinInterspawn(this Events @event)
    {
        switch (@event)
        {
            //case Events.EverynyanTriplet: return 8;
            case Events.FullWall: return 5;
            default: return 2;
        }
    }

    public static bool ProbabilityRoll(this Events @event)
    {
        switch (@event)
        {
//            case Events.EverynyanTriplet: return RandomHelper.Probability(0.25f);
            case Events.FullWall: return RandomHelper.Probability(0.625f);
            default: return true;
        }
    }
}
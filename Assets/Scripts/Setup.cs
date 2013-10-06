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

        var timeOffset = 0;
        for (int i = 0; i < 100; i++)
        {
            var randomEvent = RandomHelper.InEnum<Events>();
            while (m.ContainsKey(randomEvent))
                randomEvent = RandomHelper.InEnum<Events>();

            timeOffset += AddEvent(level, randomEvent, timeOffset) + RandomHelper.Random.Next(-1, 5);

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

    static int AddEvent(Level level, Events @event, int timeOffset)
    {
        var t = timeOffset;
        bool addEverynyan = RandomHelper.Probability(0.4);

        switch (@event)
        {
            case Events.BoxedMegaclock:
            {
                t += 3;
                var xOffset = RandomHelper.Random.Next(-4, 5);

                level.AddWall(new SpawnState { AtTime = t, Position = xOffset });
                level.AddWall(new SpawnState { AtTime = t + 1, Position = xOffset - 1 });
                level.AddClock(new ClockSpawnState { AtTime = t + 1, Position = xOffset, TimeDelay = 0, ShootEvery = 0.237f, RotateSpeed = 1.0f, Acceleration = _ => -0.00015f });
                level.AddWall(new SpawnState { AtTime = t + 1, Position = xOffset + 1 });
                level.AddWall(new SpawnState { AtTime = t + 2, Position = xOffset });
                return 8;
            }

            case Events.DrillerWave:
                level.AddDrill(new SpawnState { AtTime = t, Position = 0 });
                level.AddDrill(new SpawnState { AtTime = t + 1, Position = -7 });
                level.AddDrill(new SpawnState { AtTime = t + 2, Position = 7 });
                level.AddDrill(new SpawnState { AtTime = t + 3, Position = -7 });
                level.AddDrill(new SpawnState { AtTime = t + 4, Position = 7 });
                level.AddDrill(new SpawnState { AtTime = t + 5, Position = -7 });
                level.AddDrill(new SpawnState { AtTime = t + 6, Position = 7 });
                if (addEverynyan)
                    level.AddEverynian(new SpawnState { AtTime = t + 3, Position = 0 });
                return 0;

            case Events.Corridor:
            {
                var width = RandomHelper.Random.Next(4, 8);
                for (int i = 0; i <= 6; i++)
                {
                    float step = (i - 3) / 3.0f;

                    level.AddSingleShot(new SingleShotSpawnState { AtTime = t + i, Position = -width, ShootEvery = 1.5f, Angle = 90 + step * 25, TimeDelay = i / 6f });
                    level.AddSingleShot(new SingleShotSpawnState { AtTime = t + i, Position = width, ShootEvery = 1.5f, Angle = -90 - step * 25, TimeDelay = i / 6f });
                }
                if (addEverynyan)
                    level.AddEverynian(new EverynianState { AtTime = t + 2, Position = 0 });
                return 7;
            }

            case Events.EverynyanTriplet:
                level.AddEverynian(new EverynianState { AtTime = t, Position = -1, ForcedPowerup = 0 });
                level.AddEverynian(new EverynianState { AtTime = t, Position = -6, ForcedPowerup = 1 });
                level.AddEverynian(new EverynianState { AtTime = t, Position = 5, ForcedPowerup = 2 });
                return 5;

            case Events.FullWall:
                for (int i = -8; i < 9; i += 2)
                    level.AddWall(new SpawnState { AtTime = t, Position = i });
                return 3;

            case Events.HomingPyramid:
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 2, Position = -6, ShootEvery = 1.0f, ShootPauseTime = 2, ShootPauseOffset = 0, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 2, Position = 6, ShootEvery = 1.0f, ShootPauseTime = 2, ShootPauseOffset = 0, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = -3, ShootEvery = 1.0f, ShootPauseTime = 2, ShootPauseOffset = -1, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = 3, ShootEvery = 1.0f, ShootPauseTime = 2, ShootPauseOffset = -1, Homing = true });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 0, Position = 0, ShootEvery = 1.0f, ShootPauseTime = 2, Homing = true });
                return 4;

            case Events.Triangle:
            {
                var xOffset = RandomHelper.Random.Next(-4, 5);

                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = -1 + xOffset, ShootEvery = 1.0f, ShootPauseTime = 1, Angle = 5 });
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t + 1, Position = 1 + xOffset, ShootEvery = 1.0f, ShootPauseTime = 1, Angle = -5 });
                level.AddSingleShot(new HurterSpawnState { AtTime = t, Position = xOffset, ShootEvery = 1.0f, ShootPauseTime = 0 });
                return 3;
            }

            case Events.Rail:
            {
                var xOffset = RandomHelper.Random.Next(-6, 7);
                level.AddSingleShot(new SingleShotSpawnState { AtTime = t, Position = xOffset, ShootEvery = 3.0f, Angle = 5, Acceleration = s => Mathf.Clamp01(s - 0.75f) * 0.04f, Homing = true, Dangerous = true });
                return 2;
            }

            case Events.Walled:
                level.AddWall(new SpawnState { AtTime = t, Position = -3 });
                level.AddClock(new ClockSpawnState { AtTime = t + 1, Position = -3, ShootEvery = 0.5f, RotateSpeed = 0.5f, Clockwise = true });
                level.AddWall(new SpawnState { AtTime = t, Position = 3 });
                level.AddClock(new ClockSpawnState { AtTime = t + 1, Position = 3, ShootEvery = 0.5f, RotateSpeed = 0.5f, Clockwise = false });
                level.AddWall(new SpawnState { AtTime = t + 3, Position = -6 });
                level.AddCarrier(new HurterSpawnState { AtTime = t + 4, Position = -6, ShootEvery = 3.0f });
                level.AddWall(new SpawnState { AtTime = t + 3, Position = 6 });
                level.AddCarrier(new HurterSpawnState { AtTime = t + 4, Position = 6, ShootEvery = 3.0f });
                if (addEverynyan)
                    level.AddEverynian(new SpawnState { AtTime = t + 3, Position = 0 });

                return 6;
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
    EverynyanTriplet,
    Triangle,
    Rail,
    Corridor,
    FullWall
}


static class EventExtensions
{
    public static int GetMinInterspawn(this Events @event)
    {
        switch (@event)
        {
            case Events.EverynyanTriplet: return 8;
            case Events.FullWall: return 5;
            default: return 1;
        }
    }
}
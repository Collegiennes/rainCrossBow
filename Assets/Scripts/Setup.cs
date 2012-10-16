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

        //for (int i = 0; i < 93; i++)
        //{
        //    if (RandomHelper.Probability(0.05))
        //        level.AddEverynian(new SpawnState { Position = RandomHelper.Random.Next(-7, 8), AtTime = i });
        //    if (RandomHelper.Probability(0.2))
        //        level.AddWall(new SpawnState { Position = RandomHelper.Random.Next(-7, 8), AtTime = i });
        //    if (RandomHelper.Probability(0.2))
        //        level.AddDrill(new SpawnState { Position = RandomHelper.Random.Next(-7, 8), AtTime = i });
        //    if (RandomHelper.Probability(0.2))
        //        level.AddSingleShot(new HurterSpawnState { Position = RandomHelper.Random.Next(-7, 8), AtTime = i, ShootEvery = Random.value, ShootRatio = RandomHelper.Random.Next(1, 6) });
        //    if (RandomHelper.Probability(0.2))
        //        level.AddCarrier(new HurterSpawnState { Position = RandomHelper.Random.Next(-7, 8), AtTime = i, ShootEvery = Random.value, ShootRatio = RandomHelper.Random.Next(1, 6) });
        //    if (RandomHelper.Probability(0.1))
        //        level.AddClock(new ClockSpawnState { Position = RandomHelper.Random.Next(-7, 8), AtTime = i, ShootRatio = RandomHelper.Random.Next(1, 6), Clockwise = RandomHelper.Probability(0.5) });
        //}

        for (int i = 0; i <= 30; i++)
        {
            // Drills

            level.AddDrill(new SpawnState { Position = 5, AtTime = i * 30 });
            level.AddDrill(new SpawnState { Position = -5, AtTime = i * 30 });
            level.AddDrill(new SpawnState { Position = 3, AtTime = i * 45 });
            level.AddDrill(new SpawnState { Position = -3, AtTime = i * 45 });

            level.AddDrill(new SpawnState { Position = 0, AtTime = i * 20 });


            // Clocks

            level.AddClock(new ClockSpawnState { Position = -5, AtTime = i * 13 + 1, ShootRatio = Math.Abs(4) % 5, Clockwise = true });
            level.AddClock(new ClockSpawnState { Position = 5, AtTime = i * 13 + 1, ShootRatio = Math.Abs(4) % 5, Clockwise = false });

            // Walls	

            level.AddWall(new SpawnState { Position = 2, AtTime = i * 13 });
            level.AddWall(new SpawnState { Position = -2, AtTime = i * 13 });

            level.AddWall(new SpawnState { Position = 6, AtTime = i * 17 });
            level.AddWall(new SpawnState { Position = -6, AtTime = i * 17 });

            // Nyans

            level.AddEverynian(new SpawnState { Position = 0, AtTime = i * 10 });


            // Carriers

            level.AddCarrier(new HurterSpawnState { Position = -5, AtTime = i * 3 + 8, ShootEvery = 0.5f, ShootRatio = Math.Abs(1) % 5 });
            level.AddCarrier(new HurterSpawnState { Position = 5, AtTime = i * 3 + 8, ShootEvery = 0.5f, ShootRatio = Math.Abs(1) % 5 });

            level.AddCarrier(new HurterSpawnState { Position = -2, AtTime = i * 6 + 8, ShootEvery = 1.5f, ShootRatio = Math.Abs(1) % 5 });
            level.AddCarrier(new HurterSpawnState { Position = 2, AtTime = i * 6 + 8, ShootEvery = 1.5f, ShootRatio = Math.Abs(1) % 5 });


            level.AddCarrier(new HurterSpawnState { Position = 0, AtTime = i * 6 + 9, ShootEvery = 1.5f, ShootRatio = Math.Abs(1) % 5 });




        }




        // Part I

        level.AddSingleShot(new HurterSpawnState { Position = -5, AtTime = 45, ShootEvery = 0.5f, ShootRatio = Math.Abs(4) % 5 });
        level.AddSingleShot(new HurterSpawnState { Position = 5, AtTime = 45, ShootEvery = 0.5f, ShootRatio = Math.Abs(4) % 5 });
        level.AddSingleShot(new HurterSpawnState { Position = -3, AtTime = 47, ShootEvery = 0.2f, ShootRatio = Math.Abs(4) % 5 });
        level.AddSingleShot(new HurterSpawnState { Position = 3, AtTime = 47, ShootEvery = 0.2f, ShootRatio = Math.Abs(4) % 5 });

        level.AddCarrier(new HurterSpawnState { Position = -5, AtTime = 50, ShootEvery = 0.5f, ShootRatio = Math.Abs(2) % 5 });
        level.AddCarrier(new HurterSpawnState { Position = 5, AtTime = 50, ShootEvery = 0.5f, ShootRatio = Math.Abs(2) % 5 });

        level.AddWall(new SpawnState { Position = 5, AtTime = 59 });
        level.AddWall(new SpawnState { Position = -5, AtTime = 59 });


        level.AddWall(new SpawnState { Position = 5, AtTime = 53 });
        level.AddWall(new SpawnState { Position = -5, AtTime = 53 });

        level.AddClock(new ClockSpawnState { Position = 3, AtTime = 53, ShootRatio = Math.Abs(4) % 5, Clockwise = RandomHelper.Probability(0.5) });
        level.AddClock(new ClockSpawnState { Position = -3, AtTime = 53, ShootRatio = Math.Abs(4) % 5, Clockwise = true });




        level.AddWall(new SpawnState { Position = 3, AtTime = 56 });
        level.AddWall(new SpawnState { Position = -3, AtTime = 56 });


        level.AddWall(new SpawnState { Position = 4, AtTime = 67 });
        level.AddWall(new SpawnState { Position = -4, AtTime = 67 });

        level.AddWall(new SpawnState { Position = 2, AtTime = 67 });
        level.AddWall(new SpawnState { Position = -2, AtTime = 67 });

        level.AddClock(new ClockSpawnState { Position = 3, AtTime = 67, ShootRatio = Math.Abs(4) % 5, Clockwise = false });
        level.AddClock(new ClockSpawnState { Position = -3, AtTime = 67, ShootRatio = Math.Abs(4) % 5, Clockwise = true });


        level.AddCarrier(new HurterSpawnState { Position = -5, AtTime = 72, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });
        level.AddCarrier(new HurterSpawnState { Position = 5, AtTime = 72, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });


        level.AddCarrier(new HurterSpawnState { Position = -4, AtTime = 72, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });
        level.AddCarrier(new HurterSpawnState { Position = 4, AtTime = 72, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });


        level.AddCarrier(new HurterSpawnState { Position = -2, AtTime = 78, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });
        level.AddCarrier(new HurterSpawnState { Position = 2, AtTime = 78, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });


        level.AddCarrier(new HurterSpawnState { Position = -1, AtTime = 82, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });
        level.AddCarrier(new HurterSpawnState { Position = 1, AtTime = 82, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });

        level.AddWall(new SpawnState { Position = 0, AtTime = 92 });

        level.AddSingleShot(new HurterSpawnState { Position = -5, AtTime = 94, ShootEvery = 0.5f, ShootRatio = Math.Abs(4) % 5 });
        level.AddSingleShot(new HurterSpawnState { Position = 5, AtTime = 94, ShootEvery = 0.5f, ShootRatio = Math.Abs(4) % 5 });
        level.AddSingleShot(new HurterSpawnState { Position = -3, AtTime = 96, ShootEvery = 0.2f, ShootRatio = Math.Abs(4) % 5 });
        level.AddSingleShot(new HurterSpawnState { Position = 3, AtTime = 96, ShootEvery = 0.2f, ShootRatio = Math.Abs(4) % 5 });



        level.AddCarrier(new HurterSpawnState { Position = -5, AtTime = 96, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });
        level.AddCarrier(new HurterSpawnState { Position = 5, AtTime = 96, ShootEvery = 0.2f, ShootRatio = Math.Abs(2) % 5 });

        level.AddClock(new ClockSpawnState { Position = 3, AtTime = 100, ShootRatio = Math.Abs(4) % 5, Clockwise = RandomHelper.Probability(0.5) });
        level.AddClock(new ClockSpawnState { Position = -3, AtTime = 100, ShootRatio = Math.Abs(4) % 5, Clockwise = RandomHelper.Probability(0.5) });

        level.AddEverynian(new SpawnState { Position = 0, AtTime = 100 });
        level.AddClock(new ClockSpawnState { Position = 3, AtTime = 102, ShootRatio = Math.Abs(4) % 5, Clockwise = RandomHelper.Probability(0.5) });
        level.AddClock(new ClockSpawnState { Position = -3, AtTime = 102, ShootRatio = Math.Abs(4) % 5, Clockwise = RandomHelper.Probability(0.5) });  
        
    }
}

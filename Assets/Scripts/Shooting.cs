using System;
using UnityEngine;

class Shooting : MonoBehaviour
{
    static readonly float[] EnergyRequirements = { 4, 70, 50 };

    public static GameObject HitTemplate;
    public static GameObject ExplosionTemplate;

    IKeyboard Keyboard;

    public GameObject RainBullet;
    public GameObject BowBullet;
    public GameObject CrossBullet;

    GameObject Headz, RainHead, BowHead, CrossHead, CharginLazor;

    public int RainLevel = 1;
    public int BowLevel = 1;
    public int CrossLevel = 1;

    float RainSine;
    float SinceBow;
    bool ShootingBow;

    PlayerController Controller;

    readonly float[] EnergyLeft = new float[3];

    void Start()
    {
        Array.Copy(EnergyRequirements, EnergyLeft, 3);

        Headz = gameObject.FindChild("headz");
        RainHead = Headz.FindChild("chr_rain");
        BowHead = Headz.FindChild("chr_bow");
        CrossHead = Headz.FindChild("chr_cross");

        CharginLazor = gameObject.FindChild("vfx_charginmahlazor");

        Controller = GetComponent<PlayerController>();
        Keyboard = KeyboardManager.Instance;
        Keyboard.RegisterKeys(KeyCode.A, KeyCode.S, KeyCode.D);

        RainHead.SetActiveRecursively(true);
        BowHead.SetActiveRecursively(false);
        CrossHead.SetActiveRecursively(false);
        CharginLazor.SetActiveRecursively(false);

        ShootingBow = false;
    }

    void FixedUpdate()
    {
        if (Controller.Dead)
        {
            //CharginLazor.SetActiveRecursively(false);
            foreach (var r in CharginLazor.GetComponentsInChildren<Renderer>())
                r.enabled = false;
            return;
        }
        foreach (var r in CharginLazor.GetComponentsInChildren<Renderer>())
            r.enabled = true;

        CharginLazor.transform.localScale = new Vector3(0.2132132f, 0.2132132f, 0.2132132f) * ((BowLevel - 1) * 0.5f + 1); 
        CharginLazor.GetComponent<FlickerSize>().RefreshSize();

        if (BowLevel == 3)
            CharginLazor.FindChild("Sphere.2").renderer.material = ColorShifting.Materials["clr_Shift"];
        else
            CharginLazor.FindChild("Sphere.2").renderer.material = ColorShifting.Materials["clr_GreyPale"];

        var gamepadInput = GamepadsManager.Instance.Any;

        // Attacks
        bool rainShot = gamepadInput.RightTrigger.Value > 0.1 || gamepadInput.RightShoulder.State.IsDown() || Keyboard.GetKeyState(KeyCode.A).State.IsDown();
        bool crossShot = gamepadInput.LeftTrigger.Value > 0.1 || gamepadInput.LeftShoulder.State.IsDown() || Keyboard.GetKeyState(KeyCode.S).State.IsDown();
        bool bowShot = gamepadInput.A.State.IsDown() || gamepadInput.X.State.IsDown() || gamepadInput.B.State.IsDown() || gamepadInput.Y.State.IsDown()  || Keyboard.GetKeyState(KeyCode.D).State.IsDown();

        if (!rainShot) RainSine = 0;

        float energySpent = rainShot.AsNumeric() + bowShot.AsNumeric() + crossShot.AsNumeric();

        if (energySpent > 0)
        {
            RainHead.SetActiveRecursively(rainShot || energySpent > 1);
            BowHead.SetActiveRecursively(bowShot && energySpent == 1);
            CrossHead.SetActiveRecursively(crossShot && energySpent == 1);

            Headz.transform.rotation = Quaternion.Slerp(Headz.transform.rotation, Quaternion.AngleAxis(-30, Vector3.right), 0.2f);
        }
        else
        {
            Headz.transform.rotation = Quaternion.Slerp(Headz.transform.rotation, Quaternion.AngleAxis(45, Vector3.right), 0.2f);
        }

        CharginLazor.SetActiveRecursively(bowShot && SinceBow > 0.25f);
        SinceBow += Time.deltaTime;

        bowShot &= !ShootingBow;

        if (energySpent == 0) energySpent = 1;
        //energySpent = (float) Math.Pow(energySpent, 1.25f); // TODO : Good idea?

        EnergyLeft[0] -= 1f / energySpent;
        if (bowShot)
        {
            EnergyLeft[1] -= (1f / energySpent) * Mathf.Lerp(1, 2, Mathf.Pow((BowLevel - 1) / 2.0f, 2.0f));
        }
        EnergyLeft[2] -= 1f / energySpent;

        if (rainShot && EnergyLeft[0] <= 0)
        {
            switch (RainLevel)
            {
                case 1:
                {
                    Instantiate(RainBullet, transform.position, Quaternion.identity);
                    break;
                }
                case 2:
                {
                    Instantiate(RainBullet, transform.position - Vector3.right * 0.25f, Quaternion.identity);
                    Instantiate(RainBullet, transform.position + Vector3.right * 0.25f, Quaternion.identity);
                    break;
                }
                case 3:
                {
                    RainSine += Time.deltaTime * 25;

                    Instantiate(RainBullet, transform.position - Vector3.right * 0.25f, Quaternion.identity);
                    Instantiate(RainBullet, transform.position + Vector3.right * 0.25f, Quaternion.identity);

                    // Spread shot
                    var v = Vector3.Normalize(new Vector3((float)Math.Sin(RainSine) * 0.3f + 0.4f, 1, 0));
                    var go = (GameObject)Instantiate(RainBullet, transform.position, Quaternion.LookRotation(v) * Quaternion.AngleAxis(90, Vector3.right));
                    go.GetComponent<RainBulletBehaviour>().Direction = v;

                    v = Vector3.Normalize(new Vector3(-((float)Math.Sin(RainSine) * 0.3f + 0.4f), 1, 0));
                    go = (GameObject)Instantiate(RainBullet, transform.position, Quaternion.LookRotation(v) * Quaternion.AngleAxis(90, Vector3.right));
                    go.GetComponent<RainBulletBehaviour>().Direction = v;
                    break;
                }
            }
            EnergyLeft[0] = EnergyRequirements[0];
        }
        if (bowShot && EnergyLeft[1] <= 0)
        {
            var go = (GameObject) Instantiate(BowBullet, transform.position, Quaternion.AngleAxis(90, Vector3.forward));
            go.GetComponent<BowBulletBehaviour>().PlayerShot = true;
            EnergyLeft[1] = EnergyRequirements[1];
            ShootingBow = true;
            Wait.Until(t => t >= 0.49f * ((BowLevel - 1) / 3f + 1), () => { SinceBow = 0; ShootingBow = false; });
        }
        if (crossShot && EnergyLeft[2] <= 0)
        {
            switch (CrossLevel)
            {
                case 1:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var angle = ((i + 0.5f) - 2) / 1.5f * 40;
                        var q = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.AngleAxis(45, Vector3.right);
                        var go = (GameObject)Instantiate(CrossBullet, transform.position, q);
                        go.GetComponent<CrossBulletBehaviour>().Direction = q * Vector3.Normalize(Vector3.up);
                    }
                    break;
                }
                case 2:
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var angle = ((i + 0.5f) - 3) / 2.5f * 60;
                        var q = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.AngleAxis(45, Vector3.right);
                        var go = (GameObject)Instantiate(CrossBullet, transform.position, q);
                        go.GetComponent<CrossBulletBehaviour>().Direction = q * Vector3.Normalize(Vector3.up);
                    }
                    break;
                }
                case 3:
                {
                    for (int i = 0; i < 8; i++)
                    {
                        var angle = ((i + 0.5f) - 4) / 3.5f * 80;
                        var q = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.AngleAxis(45, Vector3.right);
                        var go = (GameObject)Instantiate(CrossBullet, transform.position, q);
                        go.GetComponent<CrossBulletBehaviour>().Direction = q * Vector3.Normalize(Vector3.up);
                        // TODO : Splash damage
                    }
                    break;
                }
            }
            EnergyLeft[2] = EnergyRequirements[2];
        }
    }
}

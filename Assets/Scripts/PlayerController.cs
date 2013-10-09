using System;
using System.Linq;
using UnityEngine;

class PlayerController : MonoBehaviour
{
    const float MovingSpeed = 5f;
    const float ScreenSize = 8f;
    const float TiltDegrees = 35;

    IKeyboard Keyboard;

    Vector3 Velocity;
    Quaternion Tilt;

    public bool Dead;

    void Start()
    {
        Keyboard = KeyboardManager.Instance;
        Keyboard.RegisterKeys(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        Quaternion destinationTilt = Quaternion.identity;

        var gamepadInput = GamepadsManager.Instance.Any;

        var lsP = gamepadInput.LeftStick.Position;
        if (lsP.sqrMagnitude != 0)
            lsP = Vector2.Lerp(lsP, lsP.normalized, lsP.sqrMagnitude);

        var horizAxis = Mathf.Clamp(lsP.x +
                (gamepadInput.DPad.Left.State.IsDown() || Keyboard.GetKeyState(KeyCode.LeftArrow).State.IsDown() ? -1 : 0) +
                (gamepadInput.DPad.Right.State.IsDown() || Keyboard.GetKeyState(KeyCode.RightArrow).State.IsDown() ? 1 : 0), -1, 1);

        var vertAxis = Mathf.Clamp(lsP.y +
                (gamepadInput.DPad.Down.State.IsDown() || Keyboard.GetKeyState(KeyCode.DownArrow).State.IsDown() ? -1 : 0) +
                (gamepadInput.DPad.Up.State.IsDown() || Keyboard.GetKeyState(KeyCode.UpArrow).State.IsDown() ? 1 : 0), -1, 1);

        if (horizAxis != 0)
        {
            Velocity += Vector3.right * Time.deltaTime * MovingSpeed * horizAxis;
            destinationTilt *= Quaternion.AngleAxis(TiltDegrees * horizAxis, Vector3.up);
        }
        if (vertAxis != 0) 
            Velocity += Vector3.up * Time.deltaTime * MovingSpeed * vertAxis;

        Tilt = Quaternion.Slerp(Tilt, destinationTilt, 0.25f);

        // Apply 
        transform.position += Velocity;
        transform.rotation = Tilt;

        // Friction
        Velocity *= 0.55f;

        // Limit
        transform.position = Vector3.Min(new Vector3(ScreenSize * Camera.main.aspect, ScreenSize, 0), transform.position);
        transform.position = Vector3.Max(new Vector3(-ScreenSize * Camera.main.aspect, -ScreenSize, 0), transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (Dead) return;

        var otherGo = other.gameObject;

        if (otherGo.tag == "Hurter" || otherGo.tag == "Enemy")
        {
            foreach (var go in FindObjectsOfType(typeof(GameObject)).Cast<GameObject>())
            {
                Enemy e = null;
                Bullet b = null;
                if (((e = go.GetComponent<Enemy>()) != null || (b = go.GetComponent<Bullet>()) != null) && go.GetComponent<FlickerOut>() == null)
                {
                    if (go != otherGo)
                    {
                        var fo = go.AddComponent<FlickerOut>();
                        fo.Upscaling = 0;
                        fo.Duration = 1;
                        fo.RandomizeRotation = false;
                    }

                    if (e != null) e.Dead = true;
                    if (b != null) b.Dead = true;
                }

                Dead = true;

                if (go.GetComponent<Level>() != null)
                {
                    Destroy(go);

                    Wait.Until(t => t > 3.5f, () =>
                                                  {
                                                      foreach (var r in GetComponentsInChildren<Renderer>())
                                                          r.enabled = true;
                                                      transform.position = new Vector3(0, -4, 0);
                                                      gameObject.GetComponent<Flyby>().Restart();
                                                  });
                }
            }

            var ogfo = otherGo.AddComponent<FlickerOut>();
            ogfo.Upscaling = 0;
            ogfo.Duration = 2.5f;
            ogfo.RandomizeRotation = false;

            var pfo = gameObject.AddComponent<FlickerOut>();
            pfo.Upscaling = 0;
            pfo.Duration = 1.5f;
            pfo.RandomizeRotation = false;
            pfo.DontKill = true;

            Camera.main.audio.Stop();
            audio.Play();

            GetComponent<Shooting>().RainLevel = GetComponent<Shooting>().CrossLevel = GetComponent<Shooting>().BowLevel = 1;
        }

        if (otherGo.tag == "Powerup" && otherGo.transform.parent.parent == null)
        {
            if (otherGo.name.Contains("bow"))
            {
                GetComponent<Shooting>().BowLevel = Mathf.Min(3, GetComponent<Shooting>().BowLevel + 1);
                if (GetComponent<Shooting>().BowLevel == 3)
                    Wait.Until(t => t >= 10, () => GetComponent<Shooting>().BowLevel = Math.Min(GetComponent<Shooting>().BowLevel, 2));
                Destroy(otherGo.transform.parent.gameObject);
            }
            else if (otherGo.name.Contains("cross"))
            {
                GetComponent<Shooting>().CrossLevel = Mathf.Min(3, GetComponent<Shooting>().CrossLevel + 1);
                if (GetComponent<Shooting>().CrossLevel == 3)
                    Wait.Until(t => t >= 8, () => GetComponent<Shooting>().CrossLevel = Math.Min(GetComponent<Shooting>().CrossLevel, 2));
                Destroy(otherGo.transform.parent.gameObject);
            }
            else if (otherGo.name.Contains("rain"))
            {
                GetComponent<Shooting>().RainLevel = Mathf.Min(3, GetComponent<Shooting>().RainLevel + 1);
                if (GetComponent<Shooting>().RainLevel == 3)
                    Wait.Until(t => t >= 6, () => GetComponent<Shooting>().RainLevel = Math.Min(GetComponent<Shooting>().RainLevel, 2));
                Destroy(otherGo.transform.parent.gameObject);
            }
        }
    }
}

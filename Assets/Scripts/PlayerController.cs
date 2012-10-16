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

        Keyboard.RegisterKey(KeyCode.LeftArrow);
        Keyboard.RegisterKey(KeyCode.RightArrow);
        Keyboard.RegisterKey(KeyCode.UpArrow);
        Keyboard.RegisterKey(KeyCode.DownArrow);
        Keyboard.RegisterKey(KeyCode.M);
        Keyboard.RegisterKey(KeyCode.N);
    }

    void FixedUpdate()
    {
        if (Dead) return;

        Quaternion destinationTilt = Quaternion.identity;

        // Movement
        if (Keyboard.GetKeyState(KeyCode.LeftArrow).State.IsDown())
        {
            Velocity += Vector3.left * Time.deltaTime * MovingSpeed;
            destinationTilt *= Quaternion.AngleAxis(TiltDegrees, Vector3.up);
        }
        if (Keyboard.GetKeyState(KeyCode.RightArrow).State.IsDown()) 
        {
            Velocity += Vector3.right * Time.deltaTime * MovingSpeed;
            destinationTilt *= Quaternion.AngleAxis(-TiltDegrees, Vector3.up);
        }
        if (Keyboard.GetKeyState(KeyCode.UpArrow).State.IsDown()) 
        {
            Velocity += Vector3.up * Time.deltaTime * MovingSpeed;
        }
        if (Keyboard.GetKeyState(KeyCode.DownArrow).State.IsDown()) 
        {
            Velocity += Vector3.down * Time.deltaTime * MovingSpeed;
        }

        if (Keyboard.GetKeyState(KeyCode.M).State == ComplexButtonState.Pressed)
            AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;

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

        if (otherGo.tag == "Powerup" && otherGo.transform.parent == null)
        {
            if (otherGo.name.Contains("bow"))
            {
                GetComponent<Shooting>().BowLevel = Mathf.Min(3, GetComponent<Shooting>().BowLevel + 1);
                Destroy(otherGo);
            }
            else if (otherGo.name.Contains("cross"))
            {
                GetComponent<Shooting>().CrossLevel = Mathf.Min(3, GetComponent<Shooting>().CrossLevel + 1);
                Destroy(otherGo);
            }
            else if (otherGo.name.Contains("rain"))
            {
                GetComponent<Shooting>().RainLevel = Mathf.Min(3, GetComponent<Shooting>().RainLevel + 1);
                Destroy(otherGo);
            }
        }
    }
}

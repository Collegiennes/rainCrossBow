using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour
{
    IGamepads Gamepads;
    IKeyboard Keyboard;
    void Start()
    {
        Gamepads = GamepadsManager.Instance;
        Keyboard = KeyboardManager.Instance;

        Keyboard.RegisterKey(KeyCode.Return);

        renderer.enabled = false;
        transform.GetChild(0).renderer.enabled = false;
        transform.GetChild(1).renderer.enabled = false;
    }

    void Update()
    {
        if (Gamepads.Any.Start.State == ComplexButtonState.Pressed ||
            Keyboard.GetKeyState(KeyCode.Return).State == ComplexButtonState.Pressed)
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                Camera.main.audio.Play();
                renderer.enabled = false;
                transform.GetChild(0).renderer.enabled = false;
                transform.GetChild(1).renderer.enabled = false;
            }
            else
            {
                Time.timeScale = 0;
                Camera.main.audio.Pause();
                renderer.enabled = true;
                transform.GetChild(0).renderer.enabled = true;
                transform.GetChild(1).renderer.enabled = true;
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

class KeyboardInstanciator : MonoBehaviour
{
    readonly Dictionary<KeyCode, TimedButtonState> keyStates = new Dictionary<KeyCode, TimedButtonState>(KeyCodeEqualityComparer.Default);
    readonly List<KeyCode> registeredKeys = new List<KeyCode>();

    public List<GameObject> Prefabs = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < Prefabs.Count; i++)
            registeredKeys.Add(KeyCode.A + i);
    }

    void Update()
    {
        var dt = Time.deltaTime;

        foreach (var key in registeredKeys)
        {
            TimedButtonState state;
            bool down = Input.GetKey(key);

            if (keyStates.TryGetValue(key, out state))
            {
                var nextState = state.NextState(down, dt);
                if (nextState != state)
                {
                    keyStates.Remove(key);
                    keyStates.Add(key, state.NextState(down, dt));
                }
            }
            else
                keyStates.Add(key, state.NextState(down, dt));
        }

        for (int i = 0; i < Prefabs.Count; i++)
            if (GetKeyState(KeyCode.A + i).State == ComplexButtonState.Pressed)
            {
                var go = (GameObject) Instantiate(Prefabs[i]);
                Wait.Until(t => t >= 0.5, () => Destroy(go));
            }

    }

    public TimedButtonState GetKeyState(KeyCode key)
    {
        TimedButtonState state;
        if (!keyStates.TryGetValue(key, out state))
            state = new TimedButtonState();

        return state;
    }
}

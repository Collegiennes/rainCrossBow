using System;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class GamepadsManager : MonoBehaviour, IGamepads
{
    static GamepadsManager instance;
    public static IGamepads Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(GamepadsManager)) as GamepadsManager;
                if (instance == null)
                    throw new InvalidOperationException("No instance in scene!");
            }
            return instance;
        }
    }
    void OnApplicationQuit()
    {
        instance = null;
    }

    readonly Dictionary<PlayerIndex, GamepadState> gamepadStates = new Dictionary<PlayerIndex, GamepadState>(PlayerIndexComparer.Default)
                                                                       {
                                                                           { PlayerIndex.One, new GamepadState(PlayerIndex.One) },
                                                                           { PlayerIndex.Two, new GamepadState(PlayerIndex.Two) },
                                                                           { PlayerIndex.Three, new GamepadState(PlayerIndex.Three) },
                                                                           { PlayerIndex.Four, new GamepadState(PlayerIndex.Four) }
                                                                       };
    readonly CombinedGamepadState any = new CombinedGamepadState();

    void Update()
    {
        var elapsed = Time.deltaTime;

        gamepadStates[PlayerIndex.One].Update(elapsed);
        gamepadStates[PlayerIndex.Two].Update(elapsed);
        gamepadStates[PlayerIndex.Three].Update(elapsed);
        gamepadStates[PlayerIndex.Four].Update(elapsed);

        First = gamepadStates[PlayerIndex.One].Connected ? gamepadStates[PlayerIndex.One] :
            gamepadStates[PlayerIndex.Two].Connected ? gamepadStates[PlayerIndex.Two] :
            gamepadStates[PlayerIndex.Three].Connected ? gamepadStates[PlayerIndex.Three] :
            gamepadStates[PlayerIndex.Four].Connected ? gamepadStates[PlayerIndex.Four] : null;

        any.Update(gamepadStates[PlayerIndex.One], gamepadStates[PlayerIndex.Two], gamepadStates[PlayerIndex.Three], gamepadStates[PlayerIndex.Four]);
    }

    public GamepadState this[PlayerIndex index]
    {
        get { return gamepadStates[index]; }
    }

    public GamepadState First { get; private set; }
    public GamepadState Any
    {
        get { return any; }
    }
}

public interface IGamepads
{
    GamepadState this[PlayerIndex index] { get; }
    GamepadState Any { get; }
    GamepadState First { get; }
}

class CombinedGamepadState : GamepadState
{
    public void Update(GamepadState first, GamepadState second, GamepadState third, GamepadState fourth)
    {
        Connected = first.Connected || second.Connected || third.Connected || fourth.Connected;
        if (!Connected) return;

        // Shoulders
        LeftShoulder = ArrayHelper.Coalesce(first.LeftShoulder, second.LeftShoulder, third.LeftShoulder, fourth.LeftShoulder);
        RightShoulder = ArrayHelper.Coalesce(first.RightShoulder, second.RightShoulder, third.RightShoulder, fourth.RightShoulder);

        // Triggers
        LeftTrigger = ArrayHelper.Coalesce(first.LeftTrigger, second.LeftTrigger, third.LeftTrigger, fourth.LeftTrigger);
        RightTrigger = ArrayHelper.Coalesce(first.RightTrigger, second.RightTrigger, third.RightTrigger, fourth.RightTrigger);

        // Buttons
        Start = ArrayHelper.Coalesce(first.Start, second.Start, third.Start, fourth.Start);
        Back = ArrayHelper.Coalesce(first.Back, second.Back, third.Back, fourth.Back);

        A = ArrayHelper.Coalesce(first.A, second.A, third.A, fourth.A);
        B = ArrayHelper.Coalesce(first.B, second.B, third.B, fourth.B);
        X = ArrayHelper.Coalesce(first.X, second.X, third.X, fourth.X);
        Y = ArrayHelper.Coalesce(first.Y, second.Y, third.Y, fourth.Y);

        // D-Pad
        DPad = ArrayHelper.Coalesce(first.DPad, second.DPad, third.DPad, fourth.DPad);

        // Thumbsticks
        LeftStick = ArrayHelper.Coalesce(first.LeftStick, second.LeftStick, third.LeftStick, fourth.LeftStick);
        RightStick = ArrayHelper.Coalesce(first.RightStick, second.RightStick, third.RightStick, fourth.RightStick);
    }

    public override void Update(float elapsed)
    {
        throw new NotSupportedException();
    }

    public override void Vibrate(VibrationMotor motor, double amount, float duration, EasingType easingType)
    {
        throw new NotSupportedException();
    }
}
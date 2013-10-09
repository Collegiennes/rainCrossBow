using System;
using UnityEngine;
using XInputDotNetPure;

public class GamepadState
{
    const float ConnectedCheckFrequency = 5;

    VibrationMotorState leftMotor, rightMotor;
    float sinceCheckedConnected = ConnectedCheckFrequency;

    protected GamepadState() { }
    public GamepadState(PlayerIndex playerIndex)
    {
        this.playerIndex = playerIndex;
    }

    protected readonly PlayerIndex? playerIndex;
    public PlayerIndex PlayerIndex { get { return playerIndex.Value; } }

    public DirectionalState DPad { get; protected set; }
    public ThumbstickState LeftStick { get; protected set; }
    public ThumbstickState RightStick { get; protected set; }

    public TimedButtonState A { get; protected set; }
    public TimedButtonState B { get; protected set; }
    public TimedButtonState X { get; protected set; }
    public TimedButtonState Y { get; protected set; }

    public TimedButtonState RightShoulder { get; protected set; }
    public TimedButtonState LeftShoulder { get; protected set; }

    public TimedAnalogButtonState RightTrigger { get; protected set; }
    public TimedAnalogButtonState LeftTrigger { get; protected set; }

    public TimedButtonState Start { get; protected set; }
    public TimedButtonState Back { get; protected set; }

    public bool Connected { get; protected set; }

    public virtual void Update(float elapsed)
    {
        sinceCheckedConnected += elapsed;
        if (sinceCheckedConnected >= ConnectedCheckFrequency)
        {
            sinceCheckedConnected -= ConnectedCheckFrequency;
            if (Application.platform.IsOSX())
                Connected = Input.GetJoystickNames().Length > (int) playerIndex;
            else if (Application.platform.IsWindows())
                Connected = GamePad.GetState(PlayerIndex).IsConnected;
        }

        if (!Connected) return;

        // XInput.NET
        if (Application.platform.IsWindows())
        {
            GamePadState gamepadState;
            try { gamepadState = GamePad.GetState(PlayerIndex, GamePadDeadZone.None); }
            catch { return; }

            // Vibration
            if (leftMotor.Active) leftMotor = UpdateMotor(leftMotor, elapsed);
            if (rightMotor.Active) rightMotor = UpdateMotor(rightMotor, elapsed);

            if (leftMotor.LastAmount != leftMotor.CurrentAmount || rightMotor.LastAmount != rightMotor.CurrentAmount)
                GamePad.SetVibration(PlayerIndex, leftMotor.CurrentAmount, rightMotor.CurrentAmount);

            // Shoulders
            LeftShoulder = LeftShoulder.NextState(gamepadState.Buttons.LeftShoulder == ButtonState.Pressed, elapsed);
            RightShoulder = RightShoulder.NextState(gamepadState.Buttons.RightShoulder == ButtonState.Pressed, elapsed);

            // Triggers
            LeftTrigger = LeftTrigger.NextState(gamepadState.Triggers.Left, elapsed);
            RightTrigger = RightTrigger.NextState(gamepadState.Triggers.Right, elapsed);

            // Buttons
            Start = Start.NextState(gamepadState.Buttons.Start == ButtonState.Pressed, elapsed);
            Back = Back.NextState(gamepadState.Buttons.Back == ButtonState.Pressed, elapsed);

            A = A.NextState(gamepadState.Buttons.A == ButtonState.Pressed, elapsed);
            B = B.NextState(gamepadState.Buttons.B == ButtonState.Pressed, elapsed);
            X = X.NextState(gamepadState.Buttons.X == ButtonState.Pressed, elapsed);
            Y = Y.NextState(gamepadState.Buttons.Y == ButtonState.Pressed, elapsed);

            // D-Pad
            DPad = DPad.NextState(gamepadState.DPad.Up == ButtonState.Pressed,
                                  gamepadState.DPad.Down == ButtonState.Pressed,
                                  gamepadState.DPad.Left == ButtonState.Pressed,
                                  gamepadState.DPad.Right == ButtonState.Pressed, elapsed);

            // Deadzone that shit
            const float DeadZone = 0.1f;

            var left = new Vector2(gamepadState.ThumbSticks.Left.X, gamepadState.ThumbSticks.Left.Y);
            var right = new Vector2(gamepadState.ThumbSticks.Right.X, gamepadState.ThumbSticks.Right.Y);

            if (Math.Abs(left.x) < DeadZone) left.x = 0; else left.x = (left.x - DeadZone * Math.Sign(left.x)) / (1 - DeadZone);
            if (Math.Abs(left.y) < DeadZone) left.y = 0; else left.y = (left.y - DeadZone * Math.Sign(left.y)) / (1 - DeadZone);
            if (Math.Abs(right.x) < DeadZone) right.x = 0; else right.x = (right.x - DeadZone * Math.Sign(right.x)) / (1 - DeadZone);
            if (Math.Abs(right.y) < DeadZone) right.y = 0; else right.y = (right.y - DeadZone * Math.Sign(right.y)) / (1 - DeadZone);

            // Thumbsticks
            LeftStick = LeftStick.NextState(left, gamepadState.Buttons.LeftStick == ButtonState.Pressed, elapsed);
            RightStick = LeftStick.NextState(right, gamepadState.Buttons.RightStick == ButtonState.Pressed, elapsed);
        }

        // Tattiebogle
        if (Application.platform.IsOSX())
        {
            string playerPrefix = "Player" + ((int)PlayerIndex + 1);

            // Shoulders
            LeftShoulder = LeftShoulder.NextState(Input.GetButton(playerPrefix + " LeftShoulder"), elapsed);
            RightShoulder = RightShoulder.NextState(Input.GetButton(playerPrefix + " RightShoulder"), elapsed);

            // Triggers
            LeftTrigger = LeftTrigger.NextState(Input.GetAxisRaw(playerPrefix + " LeftTrigger"), elapsed);
            RightTrigger = RightTrigger.NextState(Input.GetAxisRaw(playerPrefix + " RightTrigger"), elapsed);

            // Buttons
            Start = Start.NextState(Input.GetButton(playerPrefix + " Start"), elapsed);
            Back = Back.NextState(Input.GetButton(playerPrefix + " Back"), elapsed);

            A = A.NextState(Input.GetButton(playerPrefix + " A"), elapsed);
            B = B.NextState(Input.GetButton(playerPrefix + " B"), elapsed);
            X = X.NextState(Input.GetButton(playerPrefix + " X"), elapsed);
            Y = Y.NextState(Input.GetButton(playerPrefix + " Y"), elapsed);

            // D-Pad
            DPad = DPad.NextState(Input.GetButton(playerPrefix + " DPadUp"),
                                  Input.GetButton(playerPrefix + " DPadDown"),
                                  Input.GetButton(playerPrefix + " DPadLeft"),
                                  Input.GetButton(playerPrefix + " DPadRight"), elapsed);

            // Deadzone that shit
            const float DeadZone = 0.1f;

            var left = new Vector2(Input.GetAxisRaw(playerPrefix + " LeftStick X"), Input.GetAxisRaw(playerPrefix + " LeftStick Y") * -1);
            var right = new Vector2(Input.GetAxisRaw(playerPrefix + " RightStick X"), Input.GetAxisRaw(playerPrefix + " RightStick Y") * -1);

            if (Math.Abs(left.x) < DeadZone) left.x = 0; else left.x = (left.x - DeadZone * Math.Sign(left.x)) / (1 - DeadZone);
            if (Math.Abs(left.y) < DeadZone) left.y = 0; else left.y = (left.y - DeadZone * Math.Sign(left.y)) / (1 - DeadZone);
            if (Math.Abs(right.x) < DeadZone) right.x = 0; else right.x = (right.x - DeadZone * Math.Sign(right.x)) / (1 - DeadZone);
            if (Math.Abs(right.y) < DeadZone) right.y = 0; else right.y = (right.y - DeadZone * Math.Sign(right.y)) / (1 - DeadZone);

            // Thumbsticks
            LeftStick = LeftStick.NextState(left, Input.GetButton(playerPrefix + " LeftStick Press"), elapsed);
            RightStick = LeftStick.NextState(right, Input.GetButton(playerPrefix + " RightStick Press"), elapsed);
        }
    }

    static VibrationMotorState UpdateMotor(VibrationMotorState motorState, float elapsedTime)
    {
        if (motorState.ElapsedTime <= motorState.Duration)
        {
            var step = Easing.EaseIn(1 - motorState.ElapsedTime / motorState.Duration, motorState.EasingType);
            motorState.CurrentAmount = step * motorState.MaximumAmount;
        }
        else
        {
            motorState.CurrentAmount = 0;
            motorState.Active = false;
        }
        motorState.ElapsedTime += elapsedTime;

        return motorState;
    }

    public void Vibrate(VibrationMotor motor, double amount, float duration)
    {
        Vibrate(motor, amount, duration, EasingType.Step);
    }
    public virtual void Vibrate(VibrationMotor motor, double amount, float duration, EasingType easingType)
    {
        var motorState = new VibrationMotorState(amount, duration, easingType);
        switch (motor)
        {
            case VibrationMotor.LeftLow: leftMotor = motorState; break;
            case VibrationMotor.RightHigh: rightMotor = motorState; break;
        }
    }

    struct VibrationMotorState
    {
        public readonly float MaximumAmount;
        public readonly float Duration;
        public readonly EasingType EasingType;

        public bool Active;
        public float ElapsedTime;

        public float LastAmount { get; private set; }

        float currentAmount;
        public float CurrentAmount
        {
            get { return currentAmount; }
            set
            {
                LastAmount = currentAmount;
                currentAmount = value;
            }
        }

        public VibrationMotorState(double maximumAmount, float duration, EasingType easingType) : this()
        {
            Active = true;
            LastAmount = CurrentAmount = 0;
            ElapsedTime = 0;
            MaximumAmount = Mathf.Clamp01((float)maximumAmount);
            Duration = duration;
            EasingType = easingType;
        }
    }
}

public enum VibrationMotor
{
    None,
    LeftLow,
    RightHigh
}

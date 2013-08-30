using System;
using UnityEngine;

public struct ThumbstickState : IEquatable<ThumbstickState>
{
    const double InnerRoundness = 0;
    const double PressThreshold = 0.5;

    public readonly Vector2 Position;
    public readonly Vector2 SquaredPosition;
    public readonly Vector2 Movement;
    public readonly TimedButtonState Clicked;

    public readonly TimedButtonState Up;
    public readonly TimedButtonState Down;
    public readonly TimedButtonState Left;
    public readonly TimedButtonState Right;

    ThumbstickState(Vector2 position, Vector2 movement, TimedButtonState clicked, TimedButtonState up, TimedButtonState down, TimedButtonState left, TimedButtonState right)
    {
        Position = position;
        SquaredPosition = CircleToSquare(position);
        Movement = movement;
        Clicked = clicked;
        Up = up;
        Down = down;
        Left = left;
        Right = right;
    }

    internal ThumbstickState NextState(Vector2 position, bool clicked, float elapsed)
    {
        return new ThumbstickState(position, position - Position,
                                   Clicked.NextState(clicked, elapsed),
                                   Up.NextState(Mathf.Clamp01(position.y) > PressThreshold, elapsed),
                                   Down.NextState(Mathf.Clamp01(-position.y) > PressThreshold, elapsed),
                                   Left.NextState(Mathf.Clamp01(-position.x) > PressThreshold, elapsed),
                                   Right.NextState(Mathf.Clamp01(position.x) > PressThreshold, elapsed));
    }

    public bool Equals(ThumbstickState other)
    {
        return other.Position.Equals(Position) && other.Movement.Equals(Movement) && other.Clicked.Equals(Clicked) && other.Up.Equals(Up) && other.Down.Equals(Down) && other.Left.Equals(Left) && other.Right.Equals(Right);
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (obj.GetType() != typeof (ThumbstickState)) return false;
        return Equals((ThumbstickState) obj);
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int result = Position.GetHashCode();
            result = (result * 397) ^ Movement.GetHashCode();
            result = (result * 397) ^ Clicked.GetHashCode();
            result = (result * 397) ^ Up.GetHashCode();
            result = (result * 397) ^ Down.GetHashCode();
            result = (result * 397) ^ Left.GetHashCode();
            result = (result * 397) ^ Right.GetHashCode();
            return result;
        }
    }
    public static bool operator ==(ThumbstickState left, ThumbstickState right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(ThumbstickState left, ThumbstickState right)
    {
        return !left.Equals(right);
    }

    static Vector2 CircleToSquare(Vector2 point)
    {
        // Determine the theta angle
        var angle = Math.Atan2(point.y, point.x) + MathHelper.Pi;

        Vector2 squared;
        // Scale according to which wall we're clamping to
        // X+ wall
        if (angle <= MathHelper.PiOver4 || angle > 7 * MathHelper.PiOver4)
            squared = point * (float)(1 / Math.Cos(angle));
        // Y+ wall
        else if (angle > MathHelper.PiOver4 && angle <= 3 * MathHelper.PiOver4)
            squared = point * (float)(1 / Math.Sin(angle));
        // X- wall
        else if (angle > 3 * MathHelper.PiOver4 && angle <= 5 * MathHelper.PiOver4)
            squared = point * (float)(-1 / Math.Cos(angle));
        // Y- wall
        else if (angle > 5 * MathHelper.PiOver4 && angle <= 7 * MathHelper.PiOver4)
            squared = point * (float)(-1 / Math.Sin(angle));
        else throw new InvalidOperationException("Invalid angle...?");

        // Early-out for a perfect square output
        if (InnerRoundness == 0)
            return squared;

        // Find the inner-roundness scaling factor and LERP
        var length = point.magnitude;
        var factor = (float)Math.Pow(length, InnerRoundness);
        return Vector2.Lerp(point, squared, factor);
    }

    //public override string ToString()
    //{
    //    return StringHelper.ReflectToString(this);
    //}
}

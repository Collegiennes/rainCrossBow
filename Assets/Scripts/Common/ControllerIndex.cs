using System.Collections.Generic;
using XInputDotNetPure;

public class PlayerIndexComparer : IEqualityComparer<PlayerIndex>
{
    public static readonly PlayerIndexComparer Default = new PlayerIndexComparer();
    public bool Equals(PlayerIndex x, PlayerIndex y) { return x == y; }
    public int GetHashCode(PlayerIndex obj) { return (int)obj; }
}

public enum ControllerIndex
{
    None,
    One,
    Two,
    Three,
    Four,
    Any
}

public static class ControllerIndexExtensions
{
    static readonly PlayerIndex[] None = new PlayerIndex[] { };
    static readonly PlayerIndex[] One = new[] { PlayerIndex.One };
    static readonly PlayerIndex[] Two = new[] { PlayerIndex.Two };
    static readonly PlayerIndex[] Three = new[] { PlayerIndex.Three };
    static readonly PlayerIndex[] Four = new[] { PlayerIndex.Four };
    static readonly PlayerIndex[] Any = new[] { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };

    public static PlayerIndex GetPlayer(this ControllerIndex index)
    {
        switch (index)
        {
            case ControllerIndex.One:   return PlayerIndex.One;
            case ControllerIndex.Two:   return PlayerIndex.Two;
            case ControllerIndex.Three: return PlayerIndex.Three;
            case ControllerIndex.Four:  return PlayerIndex.Four;
        }
        //throw new InvalidOperationException("This controller index corresponds to no player or more than one player");
        return PlayerIndex.One;
    }

    public static IEnumerable<PlayerIndex> GetPlayers(this ControllerIndex index)
    {
        switch (index)
        {
            case ControllerIndex.None:  return None;
            case ControllerIndex.One:   return One;
            case ControllerIndex.Two:   return Two;
            case ControllerIndex.Three: return Three;
            case ControllerIndex.Four:  return Four;
            case ControllerIndex.Any:   return Any;
        }
        return None;
    }

    public static ControllerIndex ToControllerIndex(this PlayerIndex index)
    {
        switch (index)
        {
            case PlayerIndex.One:   return ControllerIndex.One;
            case PlayerIndex.Two:   return ControllerIndex.Two;
            case PlayerIndex.Three: return ControllerIndex.Three;
            case PlayerIndex.Four:  return ControllerIndex.Four;
        }
        return ControllerIndex.None;
    }
}

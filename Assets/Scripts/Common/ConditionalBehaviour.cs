using System;
using UnityEngine;
using Object = UnityEngine.Object;

class ConditionalBehaviour : MonoBehaviour
{
    float SinceAlive;

    public Action Action;
    public Condition Condition;

    void FixedUpdate()
    {
        SinceAlive += Time.deltaTime;
        if (Condition(SinceAlive))
        {
            Action();
            Destroy(gameObject);
            Action = null;
            Condition = null;
        }
    }
}

public delegate bool Condition(float elapsedSeconds);

public static class Wait
{
    public static GameObject Until(Condition condition, Action action)
    {
        var go = new GameObject("Waiter");
        var w = go.AddComponent<ConditionalBehaviour>();
        w.Condition = condition;
        w.Action = action;
        return go;
    }
}
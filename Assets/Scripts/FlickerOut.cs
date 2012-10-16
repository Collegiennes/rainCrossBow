using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

class FlickerOut : MonoBehaviour
{
    const float StartFrequency = 0.1f;

    public float Upscaling = 0.02f;
    public float Duration = 0.6f;
    public bool RandomizeRotation = true;
    public bool DontKill;

    float Timer;
    float SinceAlive;

    Renderer[] Renderers;

    void Start()
    {
        Timer = Random.value * StartFrequency;
        Renderers = GetComponentsInChildren<Renderer>();
    }

    void FixedUpdate()
    {
        Timer -= Time.deltaTime;
        SinceAlive += Time.deltaTime;

        if (Timer <= 0)
        {
            foreach (var r in Renderers)
            {
                if (r == null)
                {
                    Destroy(gameObject);
                    return;
                }
                r.enabled = !r.enabled;
            }
            Timer = Mathf.Lerp(0, StartFrequency, Mathf.Clamp01(SinceAlive / Duration));

            if (RandomizeRotation)
                transform.rotation = Random.rotation;

            transform.localScale += new Vector3(Upscaling, Upscaling, Upscaling);

            if (Timer == StartFrequency)
            {
                if (!DontKill)
                    Destroy(gameObject);
                else
                {
                    foreach (var r in Renderers)
                        r.enabled = false;
                    Destroy(this);
                }
            }
        }
    }
}

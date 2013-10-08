using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParticleSpeeder : MonoBehaviour
{
    ParticleAnimator pa;
    Color[] ca = new Color[5];

    void Start()
    {
        pa = GetComponent<ParticleAnimator>();
    }

    void FixedUpdate()
    {
        particleEmitter.minEmission = particleEmitter.maxEmission = 25 * Level.ScrollingSpeed;
        particleEmitter.localVelocity = new Vector3(0, -25 * Level.ScrollingSpeed, 0);
        particleEmitter.minEnergy = particleEmitter.maxEnergy = 2.5f / particleEmitter.localVelocity.y * -25.0f;

        var ss = Level.ScrollingSpeed;

        Color c = new Color();
        var cId = RandomHelper.Random.Next(0, 7);
        switch (cId)
        {
            case 0: c = new Color(1, 0, 0, 1.0f); break;
            case 1: c = new Color(0, 1, 0, 1.0f); break;
            case 2: c = new Color(0, 0, 1, 1.0f); break;
            case 3: c = new Color(1, 1, 0, 1.0f); break;
            case 4: c = new Color(1, 0, 1, 1.0f); break;
            case 5: c = new Color(0, 1, 1, 1.0f); break;
            case 6: c = new Color(1, 1, 1, 1.0f); break;
        }

        ca[0] = Color.Lerp(new Color(1, 1, 1, 0.1f), new Color(c.r, c.g, c.b, 0.1f), Mathf.Clamp01(Level.ScrollingSpeed - 1));
        ca[1] = Color.Lerp(new Color(1, 1, 1, 0.2f), new Color(c.r, c.g, c.b, 0.25f), Mathf.Clamp01(Level.ScrollingSpeed - 1));
        ca[2] = Color.Lerp(new Color(1, 1, 1, 0.225f), new Color(c.r, c.g, c.b, 0.3f), Mathf.Clamp01(Level.ScrollingSpeed - 1));
        ca[3] = Color.Lerp(new Color(1, 1, 1, 0.2f), new Color(c.r, c.g, c.b, 0.25f), Mathf.Clamp01(Level.ScrollingSpeed - 1));
        ca[4] = Color.Lerp(new Color(1, 1, 1, 0.1f), new Color(c.r, c.g, c.b, 0.1f), Mathf.Clamp01(Level.ScrollingSpeed - 1));

        pa.colorAnimation = ca;
    }
}

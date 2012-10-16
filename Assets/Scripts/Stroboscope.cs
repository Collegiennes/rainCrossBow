using UnityEngine;

public class Stroboscope : MonoBehaviour 
{
    public float Frequency;

    float Timer;
	
	void Start()
	{
        Timer = Random.value * Frequency;
	}
	
	void FixedUpdate() 
    {
		Timer += Time.deltaTime;
		if (Timer > Frequency)
		{
			renderer.enabled = !renderer.enabled;
			Timer -= Frequency;
		}	
	}
}

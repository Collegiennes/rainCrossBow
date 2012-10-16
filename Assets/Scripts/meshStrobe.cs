using UnityEngine;
using System.Collections;

public class meshStrobe : MonoBehaviour {

	float timer = Random.value * 0.1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > 0.07f)
		{
			renderer.enabled = !renderer.enabled;
			timer -= 0.07f;
		}	
	}
}

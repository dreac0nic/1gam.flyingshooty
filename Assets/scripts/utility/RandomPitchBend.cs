using System.Collections;
﻿using UnityEngine;

public class RandomPitchBend : MonoBehaviour
{
	public AudioSource Target;
	public bool AllowNegative = false;
	public float Distance = 0.5f;

	public void Start()
	{
		if(Target) {
			Target.pitch += Distance*(2*Random.value - 1.0f);
		}
	}
}

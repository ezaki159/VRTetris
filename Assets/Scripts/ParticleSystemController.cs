using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleSystemController {

	public static GameObject particleSystemPrefab;
	public static ParticleSystem particleSystem;
	static ParticleSystemController(){
		// Create a particle system.

		
        var go = new GameObject("Particle System");
        go.transform.Rotate(-90, 0, 0); // Rotate so the system emits upwards.
        particleSystem = go.AddComponent<ParticleSystem>();
	}

}

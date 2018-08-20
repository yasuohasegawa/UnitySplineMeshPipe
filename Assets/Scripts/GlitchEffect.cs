using UnityEngine;
using System.Collections;

public class GlitchEffect : BasePostEffect {

	public override string ShaderName
	{
		get { return "Custom/Glitch"; }
	}

	void Start() {
		Debug.Log ("glitch start");
		//Material.SetTexture ("_NoiseTex", noiseTex);
	}

	void Update() {
		//translate += translateSpeed * Time.deltaTime;
	}

	public void UpdateNoise(float val) {
		Material.SetFloat( "_NoiseVal", val );
	}

	public void UpdateDirection(float val) {
		Material.SetFloat( "_Direction", val );
	}

}

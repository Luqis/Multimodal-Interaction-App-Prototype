using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLimit : MonoBehaviour {
	[SerializeField]
	float v;
	public float min = 10f;
	public float max = -60f;
	// Update is called once per frame
	void Update () {
		v = gameObject.transform.eulerAngles.z;
		v = Mathf.Clamp(transform.eulerAngles.z, min, max);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, v);

	}
}

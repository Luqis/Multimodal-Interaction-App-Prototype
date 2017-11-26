using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchControl : MonoBehaviour {
	public Text tCount;
	GameObject gObj = null;
	Plane objPlane;
	Vector3 m0;
	int fingerCount = 0;

	Ray GenerateTouchRay(Vector3 touchPos){
		Vector3 touchPosFar = new Vector3 (touchPos.x, touchPos.y, Camera.main.farClipPlane);
		Vector3 touchPosNear = new Vector3 (touchPos.x, touchPos.y, Camera.main.nearClipPlane);
		Vector3 touchPosF = Camera.main.ScreenToWorldPoint (touchPosFar);
		Vector3 touchPosN = Camera.main.ScreenToWorldPoint (touchPosNear);
		Ray mr = new Ray (touchPosN, touchPosF - touchPosN);
		return mr;
	}


	void Update() {
		tCount.text = Input.touchCount.ToString ();

		if(Input.touchCount > 0){
			if (Input.GetTouch (fingerCount).phase == TouchPhase.Began) {
				Ray touchRay = GenerateTouchRay (Input.GetTouch (fingerCount).position);
				RaycastHit hit;

				if (Physics.Raycast (touchRay.origin, touchRay.direction, out hit)) {
					gObj = hit.transform.gameObject;
					objPlane = new Plane (Camera.main.transform.forward *= 1, gObj.transform.position);

					//Calcolo l'offset del touch
					Ray mRay = Camera.main.ScreenPointToRay (Input.GetTouch (fingerCount).position);
					float rayDistance;
					objPlane.Raycast (mRay, out rayDistance);
					m0 = gObj.transform.position - mRay.GetPoint (rayDistance);
				}
			} else if (Input.GetTouch (fingerCount).phase == TouchPhase.Moved && gObj) {
				Ray mRay = Camera.main.ScreenPointToRay (Input.GetTouch (fingerCount).position);
				float rayDistance;
				if (objPlane.Raycast (mRay, out rayDistance)) {
					//float v = gObj.transform.parent.rotation.z;
					//v = Mathf.Clamp(transform.rotation.eulerAngles.z, -60, 0);
					//Debug.Log (obj);
					//gObj.transform.parent.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, v);
					gObj.transform.parent.Rotate (0, 0, Input.GetTouch (fingerCount).deltaPosition.x * 0.2f,Space.World) ;
					//gObj.transform.position = mRay.GetPoint (rayDistance) + m0;
				}
			} else if (Input.GetTouch (fingerCount).phase == TouchPhase.Ended && gObj) {
				gObj = null;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

// Simple script to allow WASD camera controls with mouse look
public class CamController : MonoBehaviour {
	public Camera MainCamera = null;
	public Camera PortalCamera = null;
	public float LookSpeed = 1.0f;
	public float MoveSpeed = 1.0f;
	
	private void Update () {
		// Switch cameras (to view the scene from the Portal cameras perspective)
		if (Input.GetKeyDown (KeyCode.C)) {
			MainCamera.enabled = !MainCamera.enabled;
			PortalCamera.enabled = !PortalCamera.enabled;
		}

		// Simple Mouse Look
		if (Input.GetMouseButton (1)) {
			float xAxis = Input.GetAxis ("Mouse X");
			float yAxis = Input.GetAxis ("Mouse Y");

			Quaternion yRot = Quaternion.AngleAxis (xAxis * LookSpeed * Time.deltaTime, Vector3.up);
			Quaternion xRot = Quaternion.AngleAxis (-yAxis * LookSpeed * Time.deltaTime, transform.right);

			transform.localRotation = yRot * xRot * transform.localRotation;
		}
	
		// Basic Movement
		if (Input.GetKey (KeyCode.W)) {
			transform.position += (transform.forward * MoveSpeed * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.S)) {
			transform.position -= (transform.forward * MoveSpeed * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.A)) {
			transform.position -= (transform.right * MoveSpeed * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.D)) {
			transform.position += (transform.right * MoveSpeed * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.E)) {
			transform.position += (Vector3.up * MoveSpeed * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.Q)) {
			transform.position -= (Vector3.up * MoveSpeed * Time.deltaTime);
		}
	}
}
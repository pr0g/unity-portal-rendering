using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	public Camera MainCamera = null;
	// One to render to texture, and another to render normally to switch between
	public Camera[] PortalCameras = null;
	public Transform Source = null;
	public Transform Destination = null;
	public float NearClipTolerance = 0.0f;

	private void LateUpdate() {
		foreach (Camera portalCamera in PortalCameras) {
			// Calculate translation and rotation of MainCamera in Source space
			Vector3 cameraInSourceSpace = Source.InverseTransformPoint (MainCamera.transform.position);
			Quaternion cameraInSourceSpaceRot = Quaternion.Inverse (Source.rotation) * MainCamera.transform.rotation;

			// Note: Valid if PortalCamera and MainCamera are each in space of/children
			// of Source and Destination transforms respectively
			// portalCamera.transform.localPosition = cameraInSourceSpace;
			// portalCamera.transform.localRotation = cameraInSourceSpaceRot;

			// Transform Portal Camera to World Space relative to Destination transform,
			// matching the Main Camera position/orientation
			portalCamera.transform.position = Destination.TransformPoint (cameraInSourceSpace);
			portalCamera.transform.rotation = Destination.rotation * cameraInSourceSpaceRot;

			portalCamera.fieldOfView = MainCamera.fieldOfView;
			portalCamera.aspect = MainCamera.aspect;

			// Note: Optimisation to adjust the near clip as the camera gets further away from the Portal
			// portalCamera.nearClipPlane = Vector3.Magnitude(Source.position - MainCamera.transform.position) - NearClipTolerance;
		}
	}
}
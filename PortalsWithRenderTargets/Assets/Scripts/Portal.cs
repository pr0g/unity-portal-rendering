using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	public Camera MainCamera = null;
	// One to render to texture, and another to render normally to switch between (preview)
	public Camera[] PortalCameras = null;
	public Transform Source = null;
	public Transform Destination = null;
	
	// Helpers
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m) { return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); }
	public static Vector4 PosToV4(Vector3 v) { return new Vector4( v.x, v.y, v.z, 1.0f ); }
	public static Vector3 ToV3(Vector4 v) { return new Vector3( v.x, v.y, v.z ); }

	public static Vector3 ZeroV3 = new Vector3( 0.0f, 0.0f, 0.0f );
	public static Vector3 OneV3 = new Vector3( 1.0f, 1.0f, 1.0f );
	
	private void LateUpdate() {
		foreach (Camera portalCamera in PortalCameras) {
			// Rotate Source 180 degrees so PortalCamera is mirror image of MainCamera
			Matrix4x4 destinationFlipRotation = Matrix4x4.TRS(ZeroV3, Quaternion.AngleAxis(180.0f, Vector3.up), OneV3);
			Matrix4x4 sourceInvMat = destinationFlipRotation * Source.worldToLocalMatrix;

			// Calculate translation and rotation of MainCamera in Source space
			Vector3 cameraPositionInSourceSpace = ToV3(sourceInvMat * PosToV4(MainCamera.transform.position));
			Quaternion cameraRotationInSourceSpace = QuaternionFromMatrix(sourceInvMat) * MainCamera.transform.rotation;

			// Transform Portal Camera to World Space relative to Destination transform,
			// matching the Main Camera position/orientation
			portalCamera.transform.position = Destination.TransformPoint(cameraPositionInSourceSpace);
			portalCamera.transform.rotation = Destination.rotation * cameraRotationInSourceSpace;

			// Calculate clip plane for portal (for culling of objects inbetween destination camera and portal)
			Vector4 clipPlaneWorldSpace = new Vector4(Destination.forward.x, Destination.forward.y, Destination.forward.z, Vector3.Dot(Destination.position, -Destination.forward));
			Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

			// Update projection based on new clip plane
			// Note: http://aras-p.info/texts/obliqueortho.html and http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
			portalCamera.projectionMatrix = MainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
		}
	}
}
using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	public Camera MainCamera = null;
	// One to render to texture, and another to render normally to switch between (preview)
	public Camera[] PortalCameras = null;
	public Transform Source = null;
	public Transform Destination = null;
	
	private void LateUpdate() {
		foreach (Camera portalCamera in PortalCameras) {
			// Calculate translation and rotation of MainCamera in Source space
			Vector3 cameraPositionInSourceSpace = Source.InverseTransformPoint(MainCamera.transform.position);
			Quaternion cameraRotationInSourceSpace = Quaternion.Inverse(Source.rotation) * MainCamera.transform.rotation;

			// Transform Portal Camera to World Space relative to Destination transform,
			// matching the Main Camera position/orientation
			portalCamera.transform.position = Destination.TransformPoint(cameraPositionInSourceSpace);
			portalCamera.transform.rotation = Destination.rotation * cameraRotationInSourceSpace;

			// Calculate clip plane for portal (for culling of objects inbetween destination camera and portal)
			Vector4 clipPlaneWorldSpace = new Vector4( Destination.forward.x, Destination.forward.y, Destination.forward.z, Vector3.Dot( Destination.position, -Destination.forward ) );
			Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose( portalCamera.cameraToWorldMatrix ) * clipPlaneWorldSpace;

			// Update projection based on new clip plane
			// Note: http://aras-p.info/texts/obliqueortho.html and http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
			portalCamera.projectionMatrix = portalCamera.CalculateObliqueMatrix( clipPlaneCameraSpace );
		}
	}
}
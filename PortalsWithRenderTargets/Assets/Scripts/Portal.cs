using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera MainCamera;
    // One to render to texture, and another to render normally to switch between (preview)
    public Camera[] PortalCameras;
    public Transform Source;
    public Transform Destination;

    private void LateUpdate()
    {
        foreach (var portalCamera in PortalCameras)
        {
            // Rotate Source 180 degrees so PortalCamera is mirror image of MainCamera
            Matrix4x4 destinationFlipRotation =
                Matrix4x4.TRS(MathUtil.ZeroV3, Quaternion.AngleAxis(180.0f, Vector3.up), MathUtil.OneV3);
            Matrix4x4 sourceInvMat = destinationFlipRotation * Source.worldToLocalMatrix;

            // Calculate translation and rotation of MainCamera in Source space
            Vector3 cameraPositionInSourceSpace =
                MathUtil.ToV3(sourceInvMat * MathUtil.PosToV4(MainCamera.transform.position));
            Quaternion cameraRotationInSourceSpace =
                MathUtil.QuaternionFromMatrix(sourceInvMat) * MainCamera.transform.rotation;

            // Transform Portal Camera to World Space relative to Destination transform,
            // matching the Main Camera position/orientation
            portalCamera.transform.position = Destination.TransformPoint(cameraPositionInSourceSpace);
            portalCamera.transform.rotation = Destination.rotation * cameraRotationInSourceSpace;

            // Calculate clip plane for portal (for culling of objects in-between destination camera and portal)
            Vector4 clipPlaneWorldSpace =
                new Vector4(
                    Destination.forward.x,
                    Destination.forward.y,
                    Destination.forward.z,
                    Vector3.Dot(Destination.position, -Destination.forward));

            Vector4 clipPlaneCameraSpace =
                Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

            // Update projection based on new clip plane
            // Note: http://aras-p.info/texts/obliqueortho.html and http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
            portalCamera.projectionMatrix = MainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
    }
}
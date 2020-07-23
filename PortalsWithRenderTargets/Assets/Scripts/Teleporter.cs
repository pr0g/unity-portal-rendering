using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform In;
    public Transform Out;
    // Player controller
    public CamController CamController;

    private void Update()
    {
        Matrix4x4 destinationFlipRotation = Matrix4x4.TRS(
            MathUtil.ZeroV3, Quaternion.AngleAxis(180.0f, Vector3.up), MathUtil.OneV3);
        Matrix4x4 InInvMat = destinationFlipRotation * In.worldToLocalMatrix;

        Vector3 vecToCurrentPosition = CamController.transform.position - In.transform.position;
        Vector3 vecToPreviousPosition = CamController.PreviousPosition - In.transform.position;

        // Rough distance thresholds we must be within to teleport
        float sideDistance = Vector3.Dot(In.transform.right, vecToCurrentPosition);
        float frontDistance = Vector3.Dot(In.transform.forward, vecToCurrentPosition);
        float heightDistance = Vector3.Dot(In.transform.up, vecToCurrentPosition);
        float previousFrontDistance = Vector3.Dot(In.transform.forward, vecToPreviousPosition);

        // Have we just crossed the portal threshold
        if (    frontDistance < 0.0f
            &&  previousFrontDistance >= 0.0f
            &&  Mathf.Abs(sideDistance) < /*approx door_width*/ 1.0f
            &&  Mathf.Abs(heightDistance) < /*approx door_height*/ 1.2f)
        {
            // If so, transform the CamController to Out transform space

            Vector3 playerPositionInLocalSpace =
                MathUtil.ToV3(InInvMat * MathUtil.PosToV4(CamController.transform.position));
            Vector3 newPlayerPositionWorldSpace = Out.TransformPoint(playerPositionInLocalSpace);

            CamController.transform.position = newPlayerPositionWorldSpace;

            Quaternion cameraRotationInSourceSpace =
                MathUtil.QuaternionFromMatrix(InInvMat) * CamController.transform.rotation;

            CamController.transform.rotation = Out.rotation * cameraRotationInSourceSpace;
        }
    }
}

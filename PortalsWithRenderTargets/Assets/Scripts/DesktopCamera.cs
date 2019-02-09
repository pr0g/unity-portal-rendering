using UnityEngine;

[CreateAssetMenu(fileName = "Desktop Camera", menuName = "Cameras/Desktop Camera", order = 1)]
public class DesktopCamera : ScriptableObject
{
    public CommonCamera Camera;

    public bool CycleCamera()
    {
        return Input.GetKeyDown(KeyCode.C);
    }

    // Simple function to allow WASD camera controls with mouse look
    public (Quaternion Rotation, Vector3 Displacement) CalculateNextTransform(
        Vector3 right, Vector3 up, Vector3 forward)
    {
        Vector3 displacementDelta = Vector3.zero;
        Quaternion rotationDelta = Quaternion.identity;

        // Simple Mouse Look
        if (Input.GetMouseButton(1))
        {
            Vector2 scaledMouseDelta =
                new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))
                * Camera.LookSpeed;

            Quaternion yAxisRotation = Quaternion.AngleAxis(scaledMouseDelta.x, Vector3.up);
            Quaternion xAxisRotation = Quaternion.AngleAxis(-scaledMouseDelta.y, right);

            rotationDelta = yAxisRotation * xAxisRotation;
        }

        // Basic Movement
        if (Input.GetKey(KeyCode.W))
        {
            displacementDelta = forward * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            displacementDelta = -forward * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            displacementDelta = -right * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            displacementDelta = right * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            displacementDelta = Vector3.up * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            displacementDelta = -Vector3.up * Camera.MoveSpeed * Time.deltaTime;
        }

        return (rotationDelta, displacementDelta);
    }
}
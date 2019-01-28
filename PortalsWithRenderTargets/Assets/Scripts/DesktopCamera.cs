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
        Vector3 displacement = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        // Simple Mouse Look
        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("Mouse X") * Camera.LookSpeed;
            float yAxis = Input.GetAxis("Mouse Y") * Camera.LookSpeed;

            Quaternion yRot = Quaternion.AngleAxis(xAxis, Vector3.up);
            Quaternion xRot = Quaternion.AngleAxis(-yAxis, right);

            rotation = yRot * xRot;
        }

        // Basic Movement
        if (Input.GetKey(KeyCode.W))
        {
            displacement = forward * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            displacement = -forward * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            displacement = -right * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            displacement = right * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            displacement = Vector3.up * Camera.MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            displacement = -Vector3.up * Camera.MoveSpeed * Time.deltaTime;
        }

        return (rotation, displacement);
    }
}
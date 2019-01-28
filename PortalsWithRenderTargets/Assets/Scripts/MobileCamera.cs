using UnityEngine;

[CreateAssetMenu(fileName = "Mobile Camera", menuName = "Cameras/Mobile Camera", order = 1)]
public class MobileCamera : ScriptableObject
{
    public CommonCamera Camera;
    public float PanSpeed = 1.0f;
    public float ZoomSpeed = 1.0f;
    private bool Moving = false;

    // 3 finger tap cycles the camera on mobile
    public bool CycleCamera()
    {
        var taps = 0;
        Touch[] touches = Input.touches;
        foreach (var touch in touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                taps++;
            }
        }

        return taps == 3;
    }

    public (Quaternion Rotation, Vector3 Displacement) CalculateNextTransform(
        Vector3 right, Vector3 up, Vector3 forward)
    {
        Vector3 displacement = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        // Free look
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 delta = touch.deltaPosition * Camera.LookSpeed;
            Quaternion yRot = Quaternion.AngleAxis(delta.x, Vector3.up);
            Quaternion xRot = Quaternion.AngleAxis(-delta.y, right);

            rotation = yRot * xRot;

            Moving = Moving || touch.tapCount == 2;
            if (Moving)
            {
                displacement = forward * Camera.MoveSpeed * Time.deltaTime;
            }
        }
        // Panning + Zooming
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Get the sign of each touch in both x and y
            float sx0 = Mathf.Sign(touch0.deltaPosition.x);
            float sy0 = Mathf.Sign(touch0.deltaPosition.y);
            float sx1 = Mathf.Sign(touch1.deltaPosition.x);
            float sy1 = Mathf.Sign(touch1.deltaPosition.y);

            // Find the min absolute movement in x and y
            float x = Mathf.Min(
                Mathf.Abs(touch0.deltaPosition.x), Mathf.Abs(touch1.deltaPosition.x));
            float y = Mathf.Min(
                Mathf.Abs(touch0.deltaPosition.y), Mathf.Abs(touch1.deltaPosition.y));

            // Sum the signs of the x and y input, if the directions are opposite,
            // zero the input, otherwise choose sign (-1 or +1)
            float sx = Mathf.Abs(sx0 + sx1) > 0.0f ? Mathf.Sign(sx0 + sx1) : 0.0f;
            float sy = Mathf.Abs(sy0 + sy1) > 0.0f ? Mathf.Sign(sy0 + sy1) : 0.0f;

            // Pan in camera space (both touches must move in the same direction)
            Vector2 delta = new Vector2(x * sx, y * sy);
            displacement = ((right * delta.x) + (up * delta.y)) * PanSpeed;

            // Pinch/Zoom Reference - https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom

            // Find the position in the previous frame of each touch.
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            displacement -= forward * deltaMagnitudeDiff * ZoomSpeed;
        }
        else
        {
            Moving = false;
        }

        return (rotation, displacement);
    }
}
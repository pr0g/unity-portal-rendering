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

    public (Quaternion Rotation, Vector3 Displacement)
        CalculateNextTransform(Vector3 right, Vector3 up, Vector3 forward)
    {
        // Zero displacement and rotation deltas
        Vector3 displacementDelta = Vector3.zero;
        Quaternion rotationDelta = Quaternion.identity;

        // Free look
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 scaledTouchDelta = touch.deltaPosition * Camera.LookSpeed;
            Quaternion yAxisRotation = Quaternion.AngleAxis(scaledTouchDelta.x, Vector3.up);
            Quaternion xAxisRotation = Quaternion.AngleAxis(-scaledTouchDelta.y, right);

            rotationDelta = yAxisRotation * xAxisRotation;
            Moving = Moving || touch.tapCount == 2;

            if (Moving)
            {
                displacementDelta = forward * Camera.MoveSpeed * Time.deltaTime;
            }
        }
        // Panning + Zooming
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Get the sign of each touch in both x and y
            Vector2 touch0DeltaSign = new Vector2(
                Mathf.Sign(touch0.deltaPosition.x), Mathf.Sign(touch0.deltaPosition.y));
            Vector2 touch1DeltaSign = new Vector2(
                Mathf.Sign(touch1.deltaPosition.x), Mathf.Sign(touch1.deltaPosition.y));

            // Sum the signs of the x and y input of both touches
            Vector2 twoTouchDeltaSign = touch0DeltaSign + touch1DeltaSign;
            // If the directions are opposite, zero the input, otherwise choose sign (-1 or +1)
            Vector2 finalTwoTouchDeltaSign = new Vector2(
                Mathf.Abs(twoTouchDeltaSign.x) > 0.0f ? Mathf.Sign(twoTouchDeltaSign.x) : 0.0f,
                Mathf.Abs(twoTouchDeltaSign.y) > 0.0f ? Mathf.Sign(twoTouchDeltaSign.y) : 0.0f);

            // Find the min absolute movement in x and y
            Vector2 minTouchDelta = new Vector2(
                Mathf.Min(Mathf.Abs(touch0.deltaPosition.x), Mathf.Abs(touch1.deltaPosition.x)),
                Mathf.Min(Mathf.Abs(touch0.deltaPosition.y), Mathf.Abs(touch1.deltaPosition.y)));

            // Pan in camera space (both touches must move in the same direction)
            Vector2 finalInputDelta = minTouchDelta * finalTwoTouchDeltaSign;
            displacementDelta = ((right * finalInputDelta.x) + (up * finalInputDelta.y)) * PanSpeed;

            // Pinch/Zoom Reference - https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom
            {
                // Find the position in the previous frame of each touch.
                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
                float touchDeltaMag = (touch0.position - touch1.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                displacementDelta -= forward * deltaMagnitudeDiff * ZoomSpeed;
            }
        }
        else
        {
            Moving = false;
        }

        return (rotationDelta, displacementDelta);
    }
}
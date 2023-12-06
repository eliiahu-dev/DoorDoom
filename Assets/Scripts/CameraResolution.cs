using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [SerializeField] private Camera MainCamera;
    private float defaultWidth;

    private const float defaultAspect = 0.5625f;

    private void Start()
    {
        if (MainCamera.orthographic)
        {
            var cameraScale = MainCamera.orthographicSize;
            defaultWidth = cameraScale * defaultAspect;
            MainCamera.orthographicSize = defaultWidth / MainCamera.aspect;
        }
        else
        {
            var cameraScale = MainCamera.fieldOfView;
            defaultWidth = cameraScale * defaultAspect;
            var limitFOV = cameraScale < defaultWidth / MainCamera.aspect;
            MainCamera.fieldOfView = limitFOV ? defaultWidth / MainCamera.aspect : MainCamera.fieldOfView;
        }
    }
}
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CameraSwap : MonoBehaviour
{
    public ARCameraManager cameraManager; // AR 카메라 매니저 연결

    public void OnSwapButtonPressed()
    {
        // 현재 카메라가 후면(World)이면 -> 전면(User)으로 변경
        if (cameraManager.currentFacingDirection == CameraFacingDirection.World)
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.User;
        }
        // 현재 카메라가 전면(User)이면 -> 후면(World)으로 변경
        else
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.World;
        }
    }
}
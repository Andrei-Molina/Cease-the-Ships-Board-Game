using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Vector3 player1CameraLandscapePosition = new Vector3(-0.140000001f, 4.11000013f, -3.74000001f);
    private Vector3 player2CameraLandscapePosition = new Vector3(-0.140000001f, 4.11000013f, 2.6f);
    private Quaternion player1CameraLandscapeRotation = Quaternion.Euler(63.117f, 0, 0);
    private Quaternion player2CameraLandscapeRotation = Quaternion.Euler(63.117f, 180, 0);
    private Vector3 player1CameraPortraitPosition = new Vector3(-0.140000001f, 6.48999977f, -5.0999999f);
    private Vector3 player2CameraPortraitPosition = new Vector3(-0.140000001f, 6.48999977f, 4.17999983f);
    private Quaternion player1CameraPortraitRotation = Quaternion.Euler(63.117f, 0, 0);
    private Quaternion player2CameraPortraitRotation = Quaternion.Euler(63.117f, 180, 0);
    private float cameraLerpSpeed = 5f;

    private bool isPlayer1Turn;

    public bool IsPlayer1Turn
    {
        get => isPlayer1Turn;
        set => isPlayer1Turn = value;
    }

    public void HandleCameraTransition(Camera currentCamera, bool isLandscape)
    {
        if (isPlayer1Turn && isLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player1CameraLandscapePosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player1CameraLandscapeRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        else if (!isPlayer1Turn && isLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player2CameraLandscapePosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player2CameraLandscapeRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.LandscapeRight;
        }
        else if (isPlayer1Turn && !isLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player1CameraPortraitPosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player1CameraPortraitRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else if (!isPlayer1Turn && !isLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player2CameraPortraitPosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player2CameraPortraitRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.PortraitUpsideDown;
        }
    }
}

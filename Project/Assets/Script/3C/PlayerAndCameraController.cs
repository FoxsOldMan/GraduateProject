using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAndCameraController : MonoBehaviour
{

    public PlayerStateAndMovement gamePlayer;
    public CameraStateAndMovement gameCamera;
   
    private bool isActive;
    private bool isPlayerAlive;
    private bool isCameraReady;

    // Start is called before the first frame update
    void Awake()
    {
        isActive = false;
        isPlayerAlive = true;
        isCameraReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerAlive = gamePlayer.isAlive();

        if (isActive && isPlayerAlive && isCameraReady)
        {
            gamePlayer.AcceptInput();
        }
    }

    public void ActiveControl()
    {
        Debug.Log("Active");
        isActive = true;
        Debug.Log(gameCamera.state);
        if(gameCamera.state != CameraState.LockOn)
        {
            gameCamera.SetNewTarget(gamePlayer.gameObject);
            StartCoroutine(WaitUntillCameraReady());
        }
    }

    public void StopControl()
    {
        isActive = false;
        //gameCamera.SetNewTarget(null);
        //isCameraReady = false;
    }

    private IEnumerator WaitUntillCameraReady()
    {
        while(gameCamera.state != CameraState.LockOn)
        {
            Debug.Log("Camera is not ready");
            yield return new WaitForEndOfFrame();
        }

        isCameraReady = true;
        Debug.Log("Camera is ready");
        yield break;
    }
}

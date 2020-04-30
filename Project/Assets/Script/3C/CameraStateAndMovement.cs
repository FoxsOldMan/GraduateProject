using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateAndMovement : MonoBehaviour
{
    public GameObject target;
    [HideInInspector]
    public CameraState state;
    public bool largerAngle;

    private Camera gameCamera;
    private Vector3 offset;
    private float normalSize;
    

    // Start is called before the first frame update
    void Awake()
    {
        gameCamera = GetComponent<Camera>();
        if (target != null)
            state = CameraState.Translating;
        else
            state = CameraState.LostTarget;

        if (largerAngle)
            offset = new Vector3(0, 20, -5);
        else
            offset = new Vector3(0, 20, -15);

        normalSize = 12;

        Debug.Log(state);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case CameraState.LockOn:
                FollowTarget();
                break;
            case CameraState.Translating:
                TranslateToTarget();
                break;
            case CameraState.LostTarget:
                break;
            default:
                break;
        }
    }

    public void SetNewTarget(GameObject newTarget)
    {
        if(newTarget == null)
        {
            state = CameraState.LostTarget;
            return;
        }

        if(target != newTarget)
        {
            state = CameraState.Translating;
            target = newTarget;
        }
    }

    private void FollowTarget()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, 0.6f);
    }

    private void TranslateToTarget()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, 0.2f);

        if (gameCamera.orthographicSize != normalSize)
            gameCamera.orthographicSize += (normalSize - gameCamera.orthographicSize) * Time.deltaTime*1.5f;

        if ((target.transform.position + offset - transform.position).magnitude < 0.2f && normalSize - gameCamera.orthographicSize < 0.2f)
        {
            transform.position = target.transform.position + offset;
            gameCamera.orthographicSize = normalSize;

            state = CameraState.LockOn;
            Debug.Log("LockOn");
        }
    }

}

public enum CameraState
{
    LockOn,
    Translating,
    LostTarget,
}

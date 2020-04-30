using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameMode : MonoBehaviour
{
    public virtual void GameWin()
    {
        Debug.Log("Win");
    }

    public virtual void GameLose()
    {
        Debug.Log("Lose");
    }

    public virtual void GamePause()
    {
        Debug.Log("Pause");
    }
}

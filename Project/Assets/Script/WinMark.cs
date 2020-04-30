using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinMark : MonoBehaviour
{
    public BaseGameMode gameMode;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            gameMode.GameWin();
    }
}

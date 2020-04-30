using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDown : MonoBehaviour
{
    public SecondSceneGameMode gameMode;
    private NormalZombie zombie;

    // Start is called before the first frame update
    void Start()
    {
        zombie = GetComponent<NormalZombie>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!zombie.isAlive)
        {
            gameMode.EnemyDown(zombie.gameObject.name);
            this.enabled = false;
        }
    }
}

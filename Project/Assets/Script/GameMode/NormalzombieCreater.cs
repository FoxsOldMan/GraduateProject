using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalzombieCreater : MonoBehaviour
{
    public float interval;
    public GameObject normalZombie;
    public GameObject player;

    private float cdTimeLeft;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (normalZombie == null || player == null)
            return;

        if (cdTimeLeft > 0)
            cdTimeLeft -= Time.deltaTime;
        else
        {
            GameObject.Instantiate(normalZombie, transform.position, transform.rotation, null).GetComponent<NormalZombie>().SetAttackTarget(player);
            cdTimeLeft = interval;
        }
    }
}

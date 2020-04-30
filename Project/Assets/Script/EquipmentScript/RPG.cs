using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Equipment
{
    [Range(0, 10)]
    public float cdTime = 1f;
    [HideInInspector]
    public float curCDTime;

    public GameObject ammoPrefab;

    protected bool active = false;
    protected GameObject gunModel;
    protected AudioSource audioSourse;
    private Transform firePoint;
    private Transform ammoObjPool;//弹药对象池

    void Start()
    {
        firePoint = transform.Find("FirePoint");
        ammoObjPool = transform.Find("RPGAmmoObjPool");
        gunModel = transform.Find("Model").gameObject;
        handlePoint = transform.Find("HandlePoint").localPosition;
        audioSourse = GetComponent<AudioSource>();

        Close();
        gunModel.transform.localPosition -= handlePoint;
        ammoObjPool.SetParent(null);
    }

    protected override void Update()
    {
        base.Update();
        //装填冷却 
        if (curCDTime > 0f)
        {
            curCDTime -= Time.deltaTime;
            //Debug.Log(curCDTime);
        }
    }

    public override void Active()
    {
        active = true;
        gunModel.SetActive(true);
    }

    public override void Close()
    {
        active = false;
        gunModel.SetActive(false);
    }

    public override bool Work()
    {
        if (!active)
            return false;

        if (curCDTime > 0f)
        {
            Debug.Log(gameObject.name + " 冷却中，无法工作");
            return false;
        }

        Fire();
        audioSourse.Play();
        curCDTime = cdTime;
        return true;
    }

    private void Fire()
    {
        GameObject ammo;
        //若对象池中有闲置的对象，则直接调用
        if (ammoObjPool.childCount > 0)
        {
            ammo = ammoObjPool.GetChild(0).gameObject;
            if (!ammo.GetComponent<RPGAmmo>().working)
            {
                ammo.transform.position = firePoint.position;
                ammo.transform.rotation = firePoint.rotation;
                ammo.GetComponent<RPGAmmo>().Active();
                ammo.GetComponent<RPGAmmo>().master = master;
                return;
            }

        }
        //否则重新生成
        ammo = Instantiate(ammoPrefab, firePoint.position, firePoint.rotation, ammoObjPool);
        ammo.GetComponent<RPGAmmo>().Active();
        ammo.GetComponent<RPGAmmo>().master = master;
        ammo.transform.SetAsLastSibling();
    }
}

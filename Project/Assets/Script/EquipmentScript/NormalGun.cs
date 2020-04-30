using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGun : Equipment
{
    [Range(0, 2)]
    public float cdTime;
    [HideInInspector]
    public float curCDTime;

    protected bool working;
    protected GameObject gunModel;
    protected ParticleSystem fireEffect;
    protected Transform firePoint;
    protected AudioSource audioSourse;

    public override void Active()
    {
        working = true;
        gunModel.SetActive(true);
    }

    public override void Close()
    {
        working = false;
        gunModel.SetActive(false);
    }

    public override bool Work()
    {

        if (!working)
            return false;

        if (curCDTime > 0f || fireEffect.isPlaying)
        {
            Debug.Log(gameObject.name + " 冷却中，无法工作");
            return false;
        }

        fireEffect.transform.position = firePoint.position;
        fireEffect.transform.rotation = firePoint.rotation;
        fireEffect.Play();
        audioSourse.Play();
        curCDTime = cdTime;
        return true;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gunModel = transform.Find("Model").gameObject;
        handlePoint = transform.Find("HandlePoint").localPosition;
        firePoint = transform.Find("FirePoint");
        fireEffect = GetComponentInChildren<ParticleSystem>();
        audioSourse = GetComponent<AudioSource>();

        Close();   
        gunModel.transform.localPosition -= handlePoint;
        firePoint.localPosition -= handlePoint;
        fireEffect.transform.SetParent(null);
    }

    // Update is called once per frame
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

}

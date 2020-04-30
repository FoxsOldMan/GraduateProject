using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPlaceEqui : Equipment
{
    [Range(0, 2)]
    public float cdTime;
    [HideInInspector]
    public float curCDTime;

    public GameObject oilTankPrefab;
    public Material allowColor;
    public Material forbiddenColor;

    protected bool working;
    protected GameObject equiModel;
    protected MeshRenderer[] meshRenderers;
    protected AudioSource audioSource;
    protected Transform oilTankObjPool;
    protected float lastTriggerTime;
    protected bool placeable;


    public override void Active()
    {
        working = true;
        equiModel.SetActive(true);
    }

    public override void Close()
    {
        working = false;
        equiModel.SetActive(false);
    }

    public override bool Work()
    {
        if (!working || curCDTime > 0)
            return false;

        if (!placeable)
        {
            Debug.Log("放置处有障碍");
            return false;
        }
        else
        {
            Place();
            audioSource.Play();
            Debug.Log("成功放置");
            curCDTime = cdTime;
            return true;
        }


    }

    private void Place()
    {
        GameObject oilTank;
        if (oilTankObjPool.childCount > 0)
        {
            oilTank = oilTankObjPool.GetChild(0).gameObject;
            if (!oilTank.GetComponent<ExplosionEquiOnTheGround>().working)
            {
                oilTank.transform.position = equiModel.transform.position;
                oilTank.GetComponent<ExplosionEquiOnTheGround>().Active();
                oilTank.GetComponent<ExplosionEquiOnTheGround>().master = master;
                return;
            }
        }

        oilTank = Instantiate(oilTankPrefab, equiModel.transform.position, Quaternion.Euler(0, 0, 0), oilTankObjPool);
        oilTank.GetComponent<ExplosionEquiOnTheGround>().Active();
        oilTank.GetComponent<ExplosionEquiOnTheGround>().master = master;

    }

    // Start is called before the first frame update
    void Start()
    {
        equiModel = transform.Find("Model").gameObject;
        meshRenderers = equiModel.GetComponentsInChildren<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        oilTankObjPool = transform.Find("OilTankObjPool");

        Close();
        lastTriggerTime = 0;
        placeable = false;
        oilTankObjPool.transform.SetParent(null);
    }

    // Update is called once per frame
    protected override void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        //装填冷却 
        if (curCDTime > 0f)
        {
            curCDTime -= Time.deltaTime;
            //Debug.Log(curCDTime);
        }
        //更新材质
        UpdatePlaceable();
    }

    //更新材质，如果没有障碍且已冷却，可以放置，则allowColor反之forbiddenColor
    private void UpdatePlaceable()
    {
        if(curCDTime<=0 && Time.time - lastTriggerTime > 0.1)
        {
            placeable = true;
        }
        else
        {
            placeable = false;
        }

        if (placeable)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = allowColor;
            }
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = forbiddenColor;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("Light"))
            lastTriggerTime = Time.time;
    }
}

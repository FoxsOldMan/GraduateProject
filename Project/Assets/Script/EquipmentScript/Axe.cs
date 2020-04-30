using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Equipment
{
    public float damage = 10;
    protected bool working;
    protected GameObject axeModel;
    private Collider[] colliders;
    private List<Vulnerable> vulnerables;

    public override void Active()
    {
        axeModel.SetActive(true);
    }

    public override void Close()
    {
        axeModel.SetActive(false);
    }

    public override bool Work()
    {
        ActiveColliders();
        //Debug.Log("Melle Attack Start");
        return true;
    }

    public override bool Stop()
    {
        DeactiveColliders();
        //Debug.Log("Melle Attack Stop");
        return true;
    }

    private void Awake()
    {
        axeModel = transform.Find("Model").gameObject;
        handlePoint = transform.Find("HandlePoint").localPosition;
        colliders = GetComponentsInChildren<Collider>();
        axeModel.transform.localPosition -= handlePoint;
        DeactiveColliders();
        vulnerables = new List<Vulnerable>();
    }

    private void ActiveColliders()
    {
        vulnerables.Clear();
        working = true;
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }

    private void DeactiveColliders()
    {
        working = false;
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Light"))
            return;

        if (other.transform == master)
            return;

        Vulnerable vulnerable = other.GetComponentInParent<Vulnerable>();
        if(vulnerable != null && !vulnerables.Contains(vulnerable))
        {
            vulnerables.Add(vulnerable);
            if(vulnerable.GotHitted(damage, master) && vulnerable.AcceptHitBack())
            {
                Vector3 dir = other.transform.position - master.position;
                other.GetComponentInParent<Rigidbody>().AddForce(other.GetComponentInParent<Rigidbody>().mass * dir.normalized * 30, ForceMode.Impulse);
            }
        }
    }
}

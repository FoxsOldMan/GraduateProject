using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//照明
public class Illuminant
{
    public string sourceName;
    public Vector3 center;
    public float radius;

    public Illuminant(string source, Vector3 center, float radius)
    {
        sourceName = source;
        this.center = center;
        this.radius = radius;
    }

    //public void Spread()
    //{

    //}

}


public interface Photoreceptor
{
    void AddIlluminant(Illuminant illuminant);

    bool IsIlluminated();
}


//声音
public class Sound
{
    public string sourceName;
    public Vector3 center;
    public float radius;

    public void Spread()
    {

    }
}

public interface Hearable
{
    void Heard(Sound sound);
}


//接收伤害
public interface Vulnerable
{
    bool GotHitted(float DMG, Transform from);

    bool AcceptHitBack();
}


//小怪状态
public enum EnemyState
{
    StandbyOrPatrol,
    Alerted,
    Attacking
}


//装备种类
public enum EquipmentType
{
    Place,
    Melee,
    Ranged
}

//装备
public abstract class Equipment : MonoBehaviour
{
    public EquipmentType type;
    [HideInInspector]
    public Transform master;
    protected Vector3 handlePoint;

    public abstract void Active();

    public abstract void Close();

    public abstract bool Work();

    //Only for Melee
    public virtual bool Stop()
    {
        Debug.Log("Stop Melee Hit");
        return true;
    }

    protected virtual void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 90, 90);
    }
}


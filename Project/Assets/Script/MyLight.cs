using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLight : MonoBehaviour
{
    private Illuminant illuminant;

    // Start is called before the first frame update
    void Start()
    {
        illuminant = new Illuminant(gameObject.name, transform.position, 0);
    }

    private void OnTriggerStay(Collider other)
    {
        Photoreceptor obj = other.GetComponentInParent<Photoreceptor>();
        if (obj != null)
        {
            obj.AddIlluminant(illuminant);
        }

    }
}

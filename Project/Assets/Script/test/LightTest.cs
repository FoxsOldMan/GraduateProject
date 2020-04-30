using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTest : MonoBehaviour
{
    Illuminant lightTest;

    List<Photoreceptor> photoreceptors = new List<Photoreceptor>();
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject obj in objects){
            Debug.Log("name:" + obj.name);
            if(obj.GetComponent<Photoreceptor>() != null)
            {
                photoreceptors.Add(obj.GetComponent<Photoreceptor>());
            }
        }

        lightTest = new Illuminant(gameObject.name, transform.position, 0);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Photoreceptor obj in photoreceptors){
            obj.AddIlluminant(lightTest);
        }
    }
}

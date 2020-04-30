using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateUI : MonoBehaviour
{

    public Image alertImage;
    public Image attackImage;
    public Image deadImage;

    // Start is called before the first frame update
    void Start()
    {
        alertImage.enabled = false;
        alertImage.fillAmount = 1;
        attackImage.enabled = false;
        alertImage.fillAmount = 1;
        deadImage.enabled = false;
        deadImage.fillAmount = 1;

    }

    public void SetSate(EnemyState state)
    {
        alertImage.enabled = (state == EnemyState.Alerted);
        attackImage.enabled = (state == EnemyState.Attacking);
    }
    
    public void ShowDead()
    {
        alertImage.enabled = false;
        attackImage.enabled = false;
        deadImage.enabled = true;

        StartCoroutine(Close());
    }

    public void SetAlertDegree(float degree)
    {
        alertImage.fillAmount = Mathf.Clamp(degree, 0, 1);
    }

    public void SetAttackDegree(float degree)
    {
        attackImage.fillAmount = Mathf.Clamp(degree, 0, 1);
    }

    private IEnumerator Close()
    {
        //Debug.Log(alertImage.enabled);
        while(deadImage.fillAmount > 0)
        {
            deadImage.fillAmount -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield break;
    }
}

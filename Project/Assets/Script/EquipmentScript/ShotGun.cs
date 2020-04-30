using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : NormalGun
{
    public AudioClip fireVoiceClip;
    public AudioClip reloadVoiceClip;

    private Coroutine playReloadVoice;

    public override bool Work()
    {
        Debug.Log("ShotGunShot");
        StopCoroutine(PlayReloadVoice());

        audioSourse.clip = fireVoiceClip;

        if (base.Work())
        {
            StartCoroutine(PlayReloadVoice());
            return true;
        }

        return false;
    }

    private IEnumerator PlayReloadVoice()
    {
        while (audioSourse.isPlaying)
        {
            Debug.Log("wait");
            yield return new WaitForEndOfFrame();
        }

        audioSourse.clip = reloadVoiceClip;
        audioSourse.Play();
        yield break;
    }
}

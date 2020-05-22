using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public void FreezeScreen()
    {
        StartCoroutine(FreezeTransition());
    }

    public IEnumerator FreezeTransition()
    {
        var original = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = original;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class MapManagerController : MonoBehaviour
{
    public RawImage splash;

    // Use this for initialization
    void Start()
    {
        var original = splash.color;
        var c = original;
        c.a = 1;
        splash.color = c;

        splash.DOColor(original, .5f);
    }

    public void LoadLevel(string levelName)
    {
        splash.DOColor(new Color(0, 0, 0, 1), .5f)
            .OnComplete(() => SceneManager.LoadScene(levelName));
    }
}

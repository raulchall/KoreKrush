using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class MapManagerController : MonoBehaviour
{
    private RawImage splash;

    private void Awake()
    {
        splash = GameObject.Find("Splash").GetComponent<RawImage>();
    }

    // Use this for initialization
    private void Start()
    {
        splash.DOColor(Color.clear, 1);
    }

    public void LoadLevel(string levelName)
    {
        splash.DOColor(new Color(0, 0, 0, 1), .5f)
            .OnComplete(() => SceneManager.LoadScene(levelName));
    }
}

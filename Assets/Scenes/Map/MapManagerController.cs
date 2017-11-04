using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class MapManagerController : MonoBehaviour
{
    private RawImage Splash;

    private void Awake()
    {
        Splash = Instantiate(Resources.Load<GameObject>("Splash")).GetComponent<RawImage>();
    }

    // Use this for initialization
    private void Start()
    {
        Splash.DOColor(Color.clear, 1);
    }

    public void LoadLevel(string levelName)
    {
        Splash.DOColor(new Color(0, 0, 0, 1), .5f)
            .OnComplete(() => SceneManager.LoadScene(levelName));
    }
}

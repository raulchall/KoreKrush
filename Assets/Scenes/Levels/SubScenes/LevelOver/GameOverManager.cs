using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverManager : MonoBehaviour {

	Text youloose;

	// Use this for initialization
	void Start () {
		youloose = GameObject.Find ("/Canvas/YouLooseText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RetryLevel()
	{
		youloose.DOText("YOU LOOSE  :-)", 1.5f)
			.OnComplete(() => SceneManager.LoadScene("Collision"));

	}

	public void LoadLevel(string levelName)
	{
//		splash.DOColor(new Color(0, 0, 0, 1), .5f)
//			.OnComplete(() => SceneManager.LoadScene(levelName));
		SceneManager.LoadScene(levelName);
	}
}

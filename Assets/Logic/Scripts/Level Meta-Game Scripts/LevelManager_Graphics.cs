using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using KoreKrush;

public class LevelManager_Graphics : MonoBehaviour {

	public Text text;
	public Text moves;
	public Text speed_text;
	public Text distance_text;
	public RectTransform panel;
	public Scrollbar bar;

	void Awake()
	{
		KoreKrush.Events.Logic.ObjectivesUpdated += OnObjectivesUpdated;
		KoreKrush.Events.Logic.ObjectivesUiBuilt += OnObjectivesUIBuild;
		KoreKrush.Events.Logic.Warp += OnWarp_G;
		KoreKrush.Events.Logic.LevelCompleted += OnLevelCompleted;
		KoreKrush.Events.Logic.TurnsOut += OnTurnsOut;
		KoreKrush.Events.Logic.TurnsUpdated += OnTurnsUpdated;
			
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnObjectivesUpdated(BagList<Piece> list)
	{
		int i = 0;
		foreach (var item in list.list) {
			var tex = panel.GetChild (i);
			tex.GetComponent<Text>().text =  item.Key.ToString () + " " + item.Value;
		}
	}

	void OnObjectivesUIBuild(BagList<Piece> list)
	{
		int i = 0;
		foreach (var item in list.list) {
			var n = GameObject.Instantiate (text, panel);
			n.transform.SetParent(panel);
			n.fontSize = 14;
			n.GetComponent<RectTransform> ().SetPositionAndRotation(new Vector3 (50, 30 - 15 * i, 0), Quaternion.identity);

			n.text = item.Key.ToString () + " " + item.Value;

			i++;
		}
	}

	void OnWarp_G()
	{

	}

	void OnLevelCompleted()
	{
		text.text = "Ganaste Chama";
		var c = ChangeScene (3, "Test Scene");
		StartCoroutine (c);
	}

	void OnTurnsOut()
	{
		text.text = "Perdiste Chama";
		var c = ChangeScene (1.5, "Test Scene");
		StartCoroutine (c);
	}

	void OnTurnsUpdated(int turns)
	{
		moves.text = "Moves: " + turns;
	}
	 
	void ChangeScene(float time, string scene_name){
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene (scene_name);

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using KoreKrush;

public class LevelManager_Graphics : MonoBehaviour {

	Text result_text;
	Text moves;
	Text distance_text;
	RectTransform panel;
//	Canvas canvas;

	void Awake()
	{
		KoreKrush.Events.Logic.ObjectivesUpdate += OnObjectivesUpdated;
		KoreKrush.Events.Logic.ObjectivesUiBuild += OnObjectivesUIBuild;
		KoreKrush.Events.Logic.ShipWarpStart += OnWarp_G;
		KoreKrush.Events.Logic.LevelCompleted += OnLevelCompleted;
		KoreKrush.Events.Logic.TurnsOut += OnTurnsOut;
		KoreKrush.Events.Logic.TurnsUpdate += OnTurnsUpdated;
		KoreKrush.Events.Logic.PlayerDefeat += OnDefeated;

			
	}

	void OnDestroy()
	{
		KoreKrush.Events.Logic.ObjectivesUpdate -= OnObjectivesUpdated;
		KoreKrush.Events.Logic.ObjectivesUiBuild -= OnObjectivesUIBuild;
		KoreKrush.Events.Logic.ShipWarpStart -= OnWarp_G;
		KoreKrush.Events.Logic.LevelCompleted -= OnLevelCompleted;
		KoreKrush.Events.Logic.TurnsOut -= OnTurnsOut;
		KoreKrush.Events.Logic.TurnsUpdate -= OnTurnsUpdated;
		KoreKrush.Events.Logic.PlayerDefeat -= OnDefeated;
	}

	// Use this for initialization
	void Start () {
		//Text a = new Text ();
		LoadUI ();
	}
	
	// Update is called once per frame
	void Update () {
		distance_text.text = "Distance " + (LevelManager.distance_to_beat - ShipManager.traveled_distance);

	}


	void LoadUI()
	{
		result_text = GameObject.Find ("Result").GetComponent<Text>();
		moves = GameObject.Find ("Moves").GetComponent<Text>();
		distance_text = GameObject.Find ("Distance").GetComponent<Text>();
		panel = GameObject.Find ("Panel").GetComponent<RectTransform>();
//		canvas = GameObject.Find ("Canvas").GetComponent<Canvas>();

	}

	void OnObjectivesUpdated(PieceList list)
	{
		int i = 0;
		foreach (var item in list.list) {
			var tex = panel.GetComponent<RectTransform>().Find(item.Key.ToString());
			tex.GetComponent<Text>().text =  item.Key.ToString () + " " + item.Value;
			i++;
		}
	}

	void OnObjectivesUIBuild(PieceList list)
	{
		int i = 0;
		foreach (var item in list.list) {
			var n = CreateText();
			n.gameObject.GetComponent<RectTransform>().SetParent (panel);
			n.fontSize = 14;
			n.name = item.Key.ToString ();
			n.GetComponent<RectTransform> ().localPosition = new Vector3 (100, 20 - 70 * i, 0);

			n.text = item.Key.ToString () + " " + item.Value;

			i++;
		}
	}

	void OnWarp_G()
	{

	}

	void OnLevelCompleted()
	{
		result_text.text = "Ganaste Chama";
		var c = ChangeScene (2, "Map");
		StartCoroutine (c);
	}

	void OnTurnsOut()
	{
		LevelOver ();
	}

	void OnTurnsUpdated(int turns)
	{
		moves.text = "Moves: " + turns;
	}

	void OnDefeated()
	{
		LevelOver ();
	}

	Text CreateText (string s = "")
	{
		var newT = new GameObject ("label");
		newT.AddComponent<RectTransform> ();
		newT.AddComponent<CanvasRenderer> ();
		newT.AddComponent<Text> ();
		newT.layer = LayerMask.NameToLayer("UI");
		var txt = newT.GetComponent<Text> ();
		txt.text = s;
		txt.font = moves.font;

		return newT.GetComponent<Text> ();
	}
	 
	public void LevelOver()
	{
		SceneManager.LoadScene ("GameOver");
	}

	IEnumerator ChangeScene(float time, string scene_name){
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene (scene_name);

	}
}

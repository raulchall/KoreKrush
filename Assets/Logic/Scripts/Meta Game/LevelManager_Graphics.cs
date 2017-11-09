using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using KoreKrush;
using DG.Tweening;

public class LevelManager_Graphics : MonoBehaviour {

	Text result_text;
	Text moves;
	Text distance_text;
	RectTransform panel;

    RectTransform motorSkillChargerParent;
    GameObject ui_motor_element_prefab;
    GameObject motor_wrapper;
    Vector3 last_elem_pos;
    int childrencount;


	void Awake()
	{
		KoreKrush.Events.Logic.ObjectivesUpdate 	+= OnObjectivesUpdated;
		KoreKrush.Events.Logic.ObjectivesUiBuild 	+= OnObjectivesUIBuild;
		KoreKrush.Events.Logic.ShipWarpStart 		+= OnWarp_G;
		KoreKrush.Events.Logic.LevelCompleted 		+= OnLevelCompleted;
		KoreKrush.Events.Logic.TurnsOut 			+= OnTurnsOut;
		KoreKrush.Events.Logic.TurnsUpdate 			+= OnTurnsUpdated;
		KoreKrush.Events.Logic.PlayerDefeat 		+= OnDefeated;

        KoreKrush.Events.Logic.MotorSkillRestart    += RestartMotorsUI;
        KoreKrush.Events.Logic.AddMotorSkill        += AddMotorSkillUI;
        
	}
	// Destroy all events links
	void OnDestroy()
	{
		KoreKrush.Events.Logic.ObjectivesUpdate 	-= OnObjectivesUpdated;
		KoreKrush.Events.Logic.ObjectivesUiBuild 	-= OnObjectivesUIBuild;
		KoreKrush.Events.Logic.ShipWarpStart 		-= OnWarp_G;
		KoreKrush.Events.Logic.LevelCompleted 		-= OnLevelCompleted;
		KoreKrush.Events.Logic.TurnsOut 			-= OnTurnsOut;
		KoreKrush.Events.Logic.TurnsUpdate 			-= OnTurnsUpdated;
		KoreKrush.Events.Logic.PlayerDefeat 		-= OnDefeated;

        KoreKrush.Events.Logic.MotorSkillRestart    -= RestartMotorsUI;
        KoreKrush.Events.Logic.AddMotorSkill        -= AddMotorSkillUI;
    }
	// Use this for initialization
	void Start () {
		//Text a = new Text ();
		LoadUI ();
        childrencount = 0;
	}
	// Update is called once per frame
	void Update () {
		distance_text.text = "Distance Left: " + (LevelManager.distance_to_beat - ShipManager.traveled_distance);
	}


	void LoadUI()
	{
		result_text = GameObject.Find ("Result").GetComponent<Text>();
		moves = GameObject.Find ("Moves").GetComponent<Text>();
		distance_text = GameObject.Find ("Distance").GetComponent<Text>();
		panel = GameObject.Find ("Panel").GetComponent<RectTransform>();

        #region motors temporal ui
        motorSkillChargerParent = GameObject.Find("MotorUpdateZone").GetComponent<RectTransform>();
        ui_motor_element_prefab = Resources.Load("UI Prefabs/MotorSkillCharge Item") as GameObject;
        motor_wrapper = GameObject.Find("wrapper");
        #endregion
    }

	void OnObjectivesUpdated(PieceList plist)
	{
		int i = 0;
		foreach (var item in plist) {
			var tex = panel.GetComponent<RectTransform>().Find(item.Key.ToString());
			tex.GetComponent<Text>().text =  item.Key.ToString () + " " + item.Value;
			i++;
		}
	}

	void OnObjectivesUIBuild(PieceList plist)
	{
		int i = 0;
		foreach (var item in plist) {
			var n = CreateText();
			n.gameObject.GetComponent<RectTransform>().SetParent (panel);
			n.fontSize = 14;
			n.name = item.Key.ToString ();
			n.GetComponent<RectTransform> ().localPosition = new Vector3 (100, -70 * i, 0);

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

    public void RestartMotorsUI()
    {
        childrencount = 0;
        DOTween.KillAll();
        Destroy(motor_wrapper);
        motor_wrapper = new GameObject("wrapper");
        motor_wrapper.AddComponent<RectTransform>().SetParent(motorSkillChargerParent.transform);
    }

    public void AddMotorSkillUI(Motor motor, int actual_charge)
    {
        var motor_image = motor.ability.abilityImg;

        var new_elem = Instantiate(ui_motor_element_prefab, motor_wrapper.transform);
        var image = new_elem.transform.Find("Motor Image").GetComponent<Image>();
        var text = new_elem.transform.Find("Motor Charge Text").GetComponent<Text>();

        image.sprite = motor_image;
        text.text = actual_charge + "/" + motor.PowerFillCount;

        new_elem.transform.localPosition = new Vector3(-50, 500 - childrencount * 25f);

        last_elem_pos = new_elem.GetComponent<RectTransform>().position;

        var seq = DOTween.Sequence();
        seq.Append(new_elem.transform.DOMoveX(15, .5f)).AppendInterval(.5f).Append(new_elem.transform.DOMoveX(-50, .3f).SetDelay(1));
        seq.Play();
        childrencount++;
    }

	IEnumerator ChangeScene(float time, string scene_name){
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene (scene_name);

	}
}

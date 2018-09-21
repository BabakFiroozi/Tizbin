using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class StageSelection : MonoBehaviour
{
	[SerializeField] GameObject _stageObj = null;
	[SerializeField] Transform _stagesContent = null;

	[SerializeField] Button _backButton = null;

	// Use this for initialization
	void Start ()
	{
		_backButton.onClick.AddListener (() => {
			UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_MAIN_MENU);
		});

		var textAsset = Resources.Load<TextAsset> ("Levels/" + DataCarrier.Instance.GameMode.ToString ());
		var jsonObj = JSONObject.Create (textAsset.text);
		var stagesList = jsonObj.GetField ("stages").list;

		for(int i = 0; i < stagesList.Count; ++i)
		{
			var obj = Instantiate (_stageObj, _stagesContent);
			obj.transform.Find ("stage").GetComponent<Text> ().text = (i + 1).ToString ();

			int t = GamePlayerPrefs.Instance.GetPlayTime (DataCarrier.Instance.GameMode, i);
			int min = t / 60;
			int sec = t % 60;
			string timeStr = string.Format ("{0} : {1}", min, sec);

			string recordTime = GamePlayerPrefs.Instance.GetPlayRecord (DataCarrier.Instance.GameMode, i).ToString();

			obj.transform.Find ("duration").GetComponent<Text> ().text = timeStr;
			obj.transform.Find ("record").GetComponent<Text> ().text = recordTime;
			int b = i;

			bool isLocked = !GamePlayerPrefs.Instance.IsStageUnlocked (DataCarrier.Instance.GameMode, i);
			obj.transform.Find ("lock").gameObject.SetActive (isLocked);

			if (!isLocked)
				obj.GetComponent<Button> ().onClick.AddListener (() => StageButtonClick (b));
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void StageButtonClick(int index)
	{
		DataCarrier.Instance.SelectedStage = index;
		UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_GAME);
	}
}


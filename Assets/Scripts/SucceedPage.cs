using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SucceedPage : MonoBehaviour
{
	[SerializeField] Button _retryButton = null;
	[SerializeField] Button _nextButton = null;
	[SerializeField] Button _homeButton = null;
	[SerializeField] Button _leaderboardButton = null;
	[SerializeField] Text _durationText = null;
	[SerializeField] Text _wrongText = null;
	[SerializeField] Text _hintText = null;
	[SerializeField] Text _helpText = null;


	// Use this for initialization
	void Start ()
	{
		int t = GamePlayerPrefs.Instance.GetPlayTime (DataCarrier.Instance.GameMode, DataCarrier.Instance.SelectedStage);;
		int min = t / 60;
		int sec = t % 60;
		_durationText.text = string.Format ("{0:00} : {1:00}", min, sec);

		if (MemoryBoard.Instance.WrongCount > 0)
			_wrongText.text = MemoryBoard.Instance.WrongCount.ToString ();
		else
		{
			_wrongText.text = "";
			_wrongText.transform.Find ("Text").gameObject.SetActive (false);
			_wrongText.transform.Find ("Text2").gameObject.SetActive (true);
		}

		if (MemoryBoard.Instance.HintCount > 0)
			_hintText.text = MemoryBoard.Instance.HintCount.ToString ();
		else
		{
			_hintText.text = "";
			_hintText.transform.Find ("Text").gameObject.SetActive (false);
			_hintText.transform.Find ("Text2").gameObject.SetActive (true);
		}

		if (MemoryBoard.Instance.HelpCount > 0)
			_helpText.text = MemoryBoard.Instance.HelpCount.ToString ();
		else
		{
			_helpText.text = "";
			_helpText.transform.Find ("Text").gameObject.SetActive (false);
			_helpText.transform.Find ("Text2").gameObject.SetActive (true);
		}

		_retryButton.onClick.AddListener (() => {
			UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_GAME_MEMORY);
		});

		_nextButton.onClick.AddListener (() => {
			DataCarrier.Instance.SelectedStage++;
			UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_GAME_MEMORY);
		});

		_homeButton.onClick.AddListener (() => {
			UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_MAIN_MENU);
		});

		_leaderboardButton.onClick.AddListener (() => {
			gameObject.SetActive(false);
		});

	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}


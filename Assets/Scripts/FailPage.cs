using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class FailPage : MonoBehaviour
{
	[SerializeField] Button _retryButton = null;
	[SerializeField] Button _homeButton = null;
	[SerializeField] Button _recordButton = null;

	[SerializeField] Text _countText = null;
	[SerializeField] Text _hintText = null;
	[SerializeField] Text _helpText = null;


	// Use this for initialization
	void Start ()
	{
		if (SightBoard.Instance.SucceedCount > 0)
			_countText.text = SightBoard.Instance.SucceedCount.ToString ();
		else
		{
			_countText.text = "";
			_countText.transform.Find ("Text").gameObject.SetActive (false);
			_countText.transform.Find ("Text2").gameObject.SetActive (true);
		}

		if (SightBoard.Instance.HintsCount > 0)
			_hintText.text = SightBoard.Instance.HintsCount.ToString ();
		else
		{
			_hintText.text = "";
			_hintText.transform.Find ("Text").gameObject.SetActive (false);
			_hintText.transform.Find ("Text2").gameObject.SetActive (true);
		}

		if (SightBoard.Instance.HelpsCount > 0)
			_helpText.text = SightBoard.Instance.HelpsCount.ToString ();
		else
		{
			_helpText.text = "";
			_helpText.transform.Find ("Text").gameObject.SetActive (false);
			_helpText.transform.Find ("Text2").gameObject.SetActive (true);
		}

		_retryButton.onClick.AddListener (() => {
			SceneTransitor.Instance.TransitScene (SceneTransitor.SCENE_GAME_SIGHT);
		});
		_homeButton.onClick.AddListener (() => {
			SceneTransitor.Instance.TransitScene (SceneTransitor.SCENE_MAIN_MENU);
		});
		_recordButton.onClick.AddListener (() => {
			//Show leaderboard
		});
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}


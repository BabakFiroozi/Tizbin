using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
	[SerializeField] Button[] _diffButtons = null;

	[SerializeField] Button _memoryButton = null;
	[SerializeField] Button _sightButton = null;

	[SerializeField] GameObject _diffPage = null;

	bool _memSelected = false;

	// Use this for initialization
	void Start ()
	{
		GamePlayerPrefs.Instance.UnlockStage (GameModes.Easy, 0);
		GamePlayerPrefs.Instance.UnlockStage (GameModes.Normal, 0);
		GamePlayerPrefs.Instance.UnlockStage (GameModes.Hard, 0);

		_diffPage.transform.Find ("closeButton").GetComponent<Button> ().onClick.AddListener (() => {
			_diffPage.SetActive (false);
		});
		_memoryButton.onClick.AddListener (() => {
			_memSelected = true;
			_diffPage.SetActive(true);
		});
		_sightButton.onClick.AddListener (() => {
			_memSelected = false;
			_diffPage.SetActive(true);
		});

		for(int i = 0; i < _diffButtons.Length; ++i)
		{
			var button = _diffButtons [i];
			GameModes mode = (GameModes)((i + 3) * 4);
			button.onClick.AddListener (() => DiffButtonClick (mode));
		}
	}

	// Update is called once per frame
	void Update ()
	{
	}

	bool _toggleShowMemButton = false;
	bool _toggleShowSightButton = false;


	void DiffButtonClick(GameModes mode)
	{
		if (mode == GameModes.Easy) {
			/*PlayFab.ClientModels.UpdateUserDataRequest request = new PlayFab.ClientModels.UpdateUserDataRequest ();
			request.Data = new System.Collections.Generic.Dictionary<string, string> ();
			request.Data.Add ("Name", "Babak");
			request.Data.Add ("Number", "6");
			request.Data.Add ("Gender", "Male");
			PlayFab.PlayFabClientAPI.UpdateUserData (request, 
				(obj) => {
					Debug.Log("Succeed");
				},
				(obj) => {
				}
			);


			PlayFab.DataModels.SetObjectsRequest req = new PlayFab.DataModels.SetObjectsRequest (){
			};

				}, (obj) => {
			});
			return;
		}*/

		}

		DataCarrier.Instance.GameMode = mode;
		if (_memSelected)
			UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_STAGE_MENU);
		else
			UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_GAME_SIGHT);
	}
}
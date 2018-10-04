using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField] Button[] _buttons = null;

	// Use this for initialization
	void Start ()
	{
		GamePlayerPrefs.Instance.UnlockStage (GameModes.Easy, 0);
		GamePlayerPrefs.Instance.UnlockStage (GameModes.Normal, 0);
		GamePlayerPrefs.Instance.UnlockStage (GameModes.Hard, 0);
		GamePlayerPrefs.Instance.UnlockStage (GameModes.VeryHard, 0);

		for(int i = 0; i < _buttons.Length; ++i)
		{
			var button = _buttons [i];
			GameModes mode = (GameModes)((i + 3) * 4);
			button.onClick.AddListener (() => ButtonClick (mode));
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void ButtonClick(GameModes mode)
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
		UnityEngine.SceneManagement.SceneManager.LoadScene (DataCarrier.SCENE_STAGE_MENU);
	}
}
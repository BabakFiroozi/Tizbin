using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject _diffPage = null;

    [SerializeField] RectTransform _gamesContent = null;

    GameNames _selectedGame;


    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < (int)GameNames.Count; ++i)
        {
            var game = (GameNames)i;
            var elem = _gamesContent.transform.Find(game.ToString());
            var button = elem.Find("go").GetComponent<Button>();
            button.onClick.AddListener(() => SelectGame(game));
        }

        _diffPage.transform.Find("frame/close").GetComponent<Button>().onClick.AddListener(() =>
        {
            _diffPage.SetActive(false);
        });

        for (int i = 0; i < (int)GameModes.Count; ++i)
        {
            var mode = (GameModes)i;
            var button = _diffPage.transform.Find("frame").Find(mode.ToString()).GetComponent<Button>();
            button.onClick.AddListener(() => DiffButtonClick(mode));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    void SelectGame(GameNames game)
    {
        _selectedGame = game;
        _diffPage.SetActive(true);

        var animRectTr = _diffPage.transform.Find("frame").gameObject.GetComponent<RectTransform>();
        animRectTr.localScale = new Vector3(1.2f, 1.2f, 1);
        animRectTr.DOScale(1, .3f).SetEase(Ease.OutBounce);
    }

    void DiffButtonClick(GameModes mode)
    {
        if (mode == GameModes.Easy)
        {
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

        if (_selectedGame == GameNames.Memory)
            SceneTransitor.Instance.TransitScene(DataCarrier.SCENE_GAME_MEMORY);
        if (_selectedGame == GameNames.Sight)
            SceneTransitor.Instance.TransitScene(DataCarrier.SCENE_GAME_SIGHT);
        if (_selectedGame == GameNames.Math)
            SceneTransitor.Instance.TransitScene(DataCarrier.SCENE_GAME_MATH);
    }
}
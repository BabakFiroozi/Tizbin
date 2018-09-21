using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class SplashScene : MonoBehaviour
{
	bool _loadAdStarted = false;

	[SerializeField] Image _splashImage = null;

	[SerializeField] StoreNames _storeName = StoreNames.Cafebazar;

	[SerializeField] GameObject _gameMusic = null;

	void Awake()
	{
		DataCarrier.Instance.StoreName = _storeName;

//        GameAnalytics.Instance.ConfigureBuild("1.0");
//        GameAnalytics.Instance.Initialize("28bd225dbd8717aa6d0857c1f3f5d911", "ed63ea3fd0f618e36242b66c8f1263ddabb9a3fb");

    }

	// Use this for initialization
	void Start ()
	{
		bool on = GamePlayerPrefs.Instance.IsGameSoundOn();
        AudioListener.volume = on ? 1 : 0;

//        GameAnalytics.Instance.AddDesignEvent("app_start");
//        GameAnalytics.Instance.AddDesignEvent("app_open");
//
//		if (DataCarrier.Instance.StoreName != StoreNames.Bazik)
//			TaplighInterface.Instance.InitializeTapligh ("MFKSTPR2GGWFO7CPWLYFCEGQ0VGJLB");
//
//        TaplighInterface.Instance.OnLoadReadyListener = OnAdReady;

        //TaplighInterface.Instance.SetTestEnable(true);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

//		if (GameSaveData.Instance.IsFirstRun())
//        {
//          
//        }

//		OneSignal.StartInit("2cfccc36-29ef-4590-b351-bd16fa6fc70b")
//			.HandleNotificationOpened(HandleNotificationOpened)
//			.EndInit();
//		OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;

		_splashImage.DOFade (1, .5f);
		_splashImage.DOFade (0, .25f).SetDelay (1.5f);

        Invoke("GoToMenu", 3.0f);

    }

//    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
//    {
//    }

    private void OnAdReady(string unit, string token) {
		
	}

	void GoToMenu()
	{
//        GameAnalytics.Instance.AddDesignEvent("loading_main_menu");
//
//        TaplighInterface.Instance.ShowAd (DataCarrier.Tapligh_Ad_Id_1);

		if (_gameMusic != null)
		{
			DontDestroyOnLoad (_gameMusic);
			_gameMusic.GetComponent<AudioSource> ().Play ();
		}

		SceneManager.LoadScene (DataCarrier.SCENE_MAIN_MENU);
	}
	
	// Update is called once per frame
	void Update ()
	{
//        if (TaplighInterface.Instance.IsInitializeDone() && !_loadAdStarted)
//		{
//			_loadAdStarted = true;
//            TaplighInterface.Instance.LoadAd(DataCarrier.Tapligh_Ad_Id_2);
//            TaplighInterface.Instance.LoadAd(DataCarrier.Tapligh_Ad_Id_1);
//            //_debugText.text = "Is Done";
//        }
	}

	void OnDestroy()
	{
	}
}


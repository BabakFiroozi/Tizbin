using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitor : MonoBehaviour
{
	[SerializeField] TextAsset _tricksTextAsset = null;

	[SerializeField]  Image _fadeEffectBackg = null;
	[SerializeField]  float _transitTime = .3f;

	[SerializeField]  bool _transit = false;

	int _targetSceneIndex = -1;

	static SceneTransitor s_instance;
	public static SceneTransitor Instance {
		get	{ return s_instance; }
	}

	void Awake()
	{
		if(s_instance == null)
		{
			s_instance = this;
			DontDestroyOnLoad (gameObject);
			return;
		}
		else if(s_instance != this)
		{
			Destroy (gameObject);
			return;
		}
	}

	public void TransitScene(int sceneIndex, bool fade = true, bool showTrick = true)
	{
		Time.timeScale = DataCarrier.TIME_SCALE;

		float transitTime = 0;

		if(fade)
		{
			_fadeEffectBackg.gameObject.SetActive (true);
			_fadeEffectBackg.raycastTarget = true;
			_fadeEffectBackg.DOFade (1, _transitTime).SetEase(Ease.OutSine);
			transitTime = _transitTime;
		}

		if (sceneIndex == -1)
			_targetSceneIndex = SceneManager.GetActiveScene ().buildIndex;
		else
			_targetSceneIndex = sceneIndex;	

		Invoke ("LoadScene", transitTime);
	}

    void LoadScene()
    {
		DOTween.KillAll ();
		if (_targetSceneIndex != -1)
			SceneManager.LoadScene (_targetSceneIndex);

		_fadeEffectBackg.raycastTarget = false;
		_fadeEffectBackg.DOFade (0, _transitTime);
    }
}


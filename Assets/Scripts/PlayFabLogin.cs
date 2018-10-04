using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;

public class PlayFabLogin : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		if (string.IsNullOrEmpty (PlayFabSettings.TitleId))
			PlayFabSettings.TitleId = "144";

		var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true};
		PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private void OnLoginSuccess(LoginResult result)
	{
		Debug.Log("Congratulations, you made your first successful API call!");
	}

	private void OnLoginFailure(PlayFabError error)
	{
		Debug.LogWarning("Something went wrong with your first API call.  :(");
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
}


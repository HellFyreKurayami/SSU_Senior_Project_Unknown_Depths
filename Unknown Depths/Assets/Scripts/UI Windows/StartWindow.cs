using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Start Window Class
/// 
/// Most of this code is in reference to
/// a tutorial found on Linkedin Learning/Lynda
/// Tutorial by Jesse Freeman
/// 
/// https://www.linkedin.com/learning/unity-5-2d-advanced-ui/
/// </summary>

public class StartWindow : GenericWindow {

	public Button continueButton;

	public override void Open ()
	{
		var canContinue = true;

		continueButton.gameObject.SetActive (canContinue);

		if (continueButton.gameObject.activeSelf) {
			firstSelected = continueButton.gameObject;
		}


		base.Open ();
	}

	public void NewGame(){
		OnNextWindow ();
	}

	public void Continue(){
		Debug.Log ("Continue Pressed");
	}

	public void Options(){
		Debug.Log ("Options Pressed");
	}


	
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Keyboard Window Class
/// 
/// Most of this code is in reference to
/// a tutorial found on Linkedin Learning/Lynda
/// Tutorial by Jesse Freeman
/// 
/// https://www.linkedin.com/learning/unity-5-2d-advanced-ui/
/// </summary>
/// 
public class KeyboardWindow : GenericWindow {

	public Text inputField;
	public int maxCharacters = 7;

	private float delay = 0;
	private float curserDelay = .5f;
	private bool blink;
	private string _text = "";

	void Update(){
		var text = _text;

		if (_text.Length < maxCharacters) {

			text += "_";

			if (blink) {
				text = text.Remove (text.Length - 1);
			}
		}

		inputField.text = text;

		delay += Time.deltaTime;
		if (delay > curserDelay) {
			delay = 0;
			blink = !blink;
		}

	}

	public void OnKeyPress(string key){
		if (_text.Length < maxCharacters) {
			_text += key;
		}
	}

	public void OnDelete(){
		if (_text.Length > 0) {
			_text = _text.Remove (_text.Length - 1);
		}
	}

	public void OnAccept(){
		OnNextWindow ();
	}

	public void OnCancel(){
		OnPreviousWindow ();
	}

}

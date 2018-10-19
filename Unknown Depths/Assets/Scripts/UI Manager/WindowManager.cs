using UnityEngine;
using System.Collections;

/// <summary>
/// Window Manager Class
/// 
/// Most of this code is in reference to
/// a tutorial found on Linkedin Learning/Lynda
/// Tutorial by Jesse Freeman
/// 
/// https://www.linkedin.com/learning/unity-5-2d-advanced-ui/
/// </summary>

public class WindowManager : MonoBehaviour {

	[HideInInspector]
	public GenericWindow[] windows;
	public int currentWindowID;
	public int defaultWindowID;

	public GenericWindow GetWindow(int value){
		return windows [value];
	}

	private void ToggleVisability(int value){
		var total = windows.Length;

		for (var i = 0; i < total; i++) {
			var window = windows [i];
			if (i == value)
				window.Open ();
			else if (window.gameObject.activeSelf)
				window.Close ();
		}
	}

	public GenericWindow Open(int value){
		if (value < 0 || value >= windows.Length)
			return null;

		currentWindowID = value;

		ToggleVisability (currentWindowID);

		return GetWindow (currentWindowID);
	}

	void Start(){
		GenericWindow.manager = this;
		Open (defaultWindowID);
	}
}

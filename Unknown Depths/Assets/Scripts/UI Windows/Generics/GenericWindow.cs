using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Generic Window Class
/// 
/// Most of this code is in reference to
/// a tutorial found on Linkedin Learning/Lynda
/// Tutorial by Jesse Freeman
/// 
/// https://www.linkedin.com/learning/unity-5-2d-advanced-ui/
/// </summary>

public class GenericWindow : MonoBehaviour {

	public static WindowManager manager;
	public Windows nextWindow;
	public Windows previousWindow;

	public GameObject firstSelected;

	protected EventSystem eventSystem{
		get { return GameObject.Find ("EventSystem").GetComponent<EventSystem> (); }
	}

	public virtual void OnFocus(){
		eventSystem.SetSelectedGameObject (firstSelected);
	}

	protected virtual void Display(bool value){
		gameObject.SetActive (value);
	}

	public virtual void Open(){
		Display (true);
		OnFocus ();
	}

	public virtual void Close(){
		Display (false);
	}

	protected virtual void Awake () {
		Close ();
	}

	public void OnNextWindow(){
		manager.Open ((int)nextWindow - 1);
	}

	public void OnPreviousWindow(){
		manager.Open ((int)previousWindow - 1);
	}
}

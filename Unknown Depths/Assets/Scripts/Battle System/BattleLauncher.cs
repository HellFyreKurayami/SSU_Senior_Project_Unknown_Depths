using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLauncher : MonoBehaviour {

    public List<Entity> Players { get; set; }
    public List<Entity> Enemies { get; set; }

    void Awake () {

        DontDestroyOnLoad(this);
		
	}
	
	public void PrepareBattle(List<Entity> c, List<Entity> e)
    {
        Players = c;
        Enemies = e;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Combat");
    }

    public void Launch()
    {
        BattleController.BATTLE_CONTROLLER.StartBattle(Players, Enemies);
    }

    private void ReturnToMap()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("World");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLauncherDemo : MonoBehaviour {

    [SerializeField]
    private List<Entity> Players = null, Enemies = null;
    [SerializeField]
    private BattleLauncher launcher;

	public void Launch()
    {
        launcher.PrepareBattle(Players, Enemies);
    }
}

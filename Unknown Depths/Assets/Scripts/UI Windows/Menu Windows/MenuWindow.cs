using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWindow : GenericWindow
{
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            MapMaker.Instance.ToggleMovement(true);
            Close();
        }
    }

    public override void Open()
    {
        foreach(Entity player in MapMaker.Instance.Players)
        {
            player.LoadData();
        }
        base.Open();
    }

    public void PartyStats()
    {
        manager.Open((int)Windows.PartyStatsMenu - 1, false);
        Close();
    }

    public void PartySkills()
    {
        manager.Open((int)Windows.PartySkillsMenu - 1, false);
        Close();
    }

    public void ExitGame()
    {
        manager.Open((int)Windows.ExitGameConfirm - 1, false);
        Close();
    }
}

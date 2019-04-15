using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : GenericWindow {

    public override void Open()
    {
        base.Open();
    }

    public void DisplayCredits()
    {
        manager.Open((int)Windows.CreditsWindow - 1, true);
    }

    public void DisplayControls()
    {
        manager.Open((int)Windows.ControlsWindow - 1, true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        OnNextWindow();
    }

}

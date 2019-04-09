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

    public void ExitGame()
    {
        manager.Open((int)Windows.ExitGameConfirm - 1, false);
        Close();
    }
}

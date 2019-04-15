using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameConfirm : GenericWindow
{
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            manager.Open((int)Windows.MenuWindow - 1, false);
            Close();
        }
    }

    public void ConfirmExit()
    {
        manager.Open((int)Windows.StartWindow - 1, true);
    }
}

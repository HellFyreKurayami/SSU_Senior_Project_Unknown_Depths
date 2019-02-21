using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : GenericWindow {

    public override void Open()
    {
        base.Open();
    }

    public void NewGame()
    {
        OnNextWindow();
    }

}

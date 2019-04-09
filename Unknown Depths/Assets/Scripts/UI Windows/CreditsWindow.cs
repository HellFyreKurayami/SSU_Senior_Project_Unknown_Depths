using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsWindow : GenericWindow
{

    public void BackToStart()
    {
        manager.Open((int)Windows.StartWindow - 1, true);
    }
    
}

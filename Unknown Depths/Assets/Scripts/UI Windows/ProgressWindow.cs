using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
public class ProgressWindow : GenericWindow
{
    public Text value;

    public void NextOrPrevious(bool yn)
    {
        //If true, display next text
        if (yn)
        {
            value.text = "Proceed to Next Floor?";
        }
        else
        {
            value.text = "Return to Previous Floor?";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class FloorWindow : GenericWindow {

    public Text value;

	public void UpdateFloor(int floor)
    {
        if(value == null)
        {
            return;
        }

        var sb = new StringBuilder();
        sb.Append("Floor: ");
        sb.Append(floor.ToString ("D2"));

        value.text = sb.ToString();
    }
}

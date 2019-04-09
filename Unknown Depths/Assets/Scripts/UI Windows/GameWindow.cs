using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWindow : GenericWindow {

    private MapMaker mapMaker;

    protected override void Awake()
    {
        mapMaker = GetComponent<MapMaker>();
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetButton("Menu"))
        {
            MapMaker.Instance.ToggleMovement(false);
            manager.Open((int)Windows.MenuWindow - 1, false);
        }
    }

    public override void Open()
    {
        base.Open();
        mapMaker.Reset();
        Camera.main.GetComponent<MoveCamera>().enabled = true;
    }

    public override void Close()
    {
        base.Close();
        mapMaker.Shutdown();
        Camera.main.GetComponent<MoveCamera>().enabled = false;
    }
}
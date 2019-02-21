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
    }

}

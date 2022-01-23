using Godot;
using System;

public class Main : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private Button btnStart;
    private Button btnStartAuto;

    public override void _Ready()
    {
        btnStart = GetNode<Button>("BtnStart");
        btnStartAuto = GetNode<Button>("BtnStartAuto");
    }

    private float _updateRate = 1.0f;
    private int _frameCount;

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        ProcessFrameTime.Elapsed = delta * _updateRate;
        ProcessFrame.MoveStep();
        if (++_frameCount > 30)
        {
            _frameCount = 0;
            Console.Clear();
            Console.Write(ProcessFrame.CallStack());
        }
    }

    private void StartRun(bool autoFeed)
    {
        var machine = GetNode<Machine>("Machine");
        //var runAuto = GetNode<AutoRun>("AutoRun");
        var runAuto = GetNode<AutoRun2>("AutoRun2");

        btnStart.Disabled = btnStartAuto.Disabled = true;
        runAuto.SetAutoFeed(autoFeed);

        ProcessFrame.Emit(runAuto.StartRun(machine)).ContinueWith(() =>
        {
            btnStart.Disabled = btnStartAuto.Disabled = false;
        });
    }

    public void _on_BtnStart_pressed()
    {
        StartRun(false);
    }

    public void _on_BtnFeedOnce_pressed()
    {
        var runAuto = GetNode<AutoRun2>("AutoRun2");
        runAuto.FeedPanel(true);
    }

    public void _on_BtnStop_pressed()
    {
        var runAuto = GetNode<AutoRun2>("AutoRun2");
        runAuto.FeedPanel(false);
    }

    public void _on_BtnStartAuto_pressed()
    {
        StartRun(true);
    }

    public void _on_Btn0x_pressed()
    {
        _updateRate = 0.0f;
    }

    public void _on_Btn1x_pressed()
    {
        _updateRate = 1.0f;
    }
    public void _on_Btn2x_pressed()
    {
        _updateRate = 2.0f;
    }
    public void _on_Btn05x_pressed()
    {
        _updateRate = 0.5f;
    }

}

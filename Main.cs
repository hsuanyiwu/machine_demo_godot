using Godot;
using System;

public class Main : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
    }

    public void _on_BtnStart_pressed()
    {
        var machine = GetNode<Machine>("Machine");
        var runAuto = GetNode<AutoRun>("AutoRun");
        ProcessFrame.Emit(runAuto.StartRun(machine));
    }

    public void _on_BtnFeedOnce_pressed()
    {

    }
}

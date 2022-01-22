using Godot;
using System;

public class Machine : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    private int _frameCount;

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        ProcessFrameTime.Elapsed = delta;
        ProcessFrame.MoveStep();
        if (++_frameCount > 30)
        {
            Console.Clear();
            _frameCount = 0;
            Console.Write(ProcessFrame.CallStack());
        }
    }

    public InputPNP InputPNP()
    {
        return GetNode<InputPNP>("InputPNP");
    }

    public Car CarA()
    {
        return GetNode<Car>("CarA");
    }
}
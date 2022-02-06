using Godot;
using System;

public class Car : ColorRect
{
    [Export]
    public float _yPosIn;
    [Export]
    public float _yPosOut;

    [Export]
    public float _yPosPark;

    [Export]
    public float _yPosScan;

    private Acuator _yMove;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _yMove = GetNode<Acuator>("Acuator");
        _yMove.SetMotorPosition(_yPosPark);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public MoveTask ToPanelIn()
    {
        return _yMove.MoveTo(_yPosIn);
    }

    public MoveTask ToPanelOut()
    {
        return _yMove.MoveTo(_yPosOut);
    }

    public MoveTask Suck()
    {
        return GetNode<Sucker>("Acuator/Platform").Suck();
    }

    public MoveTask Blow()
    {
        return GetNode<Sucker>("Acuator/Platform").Blow();
    }

    public MoveTask DoScan()
    {
        return MoveTask.Create((p) =>
        {
            p.Exit();
        });
    }
}

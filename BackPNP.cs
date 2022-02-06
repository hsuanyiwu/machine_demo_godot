using Godot;
using System;

public class BackPNP : ColorRect
{
    [Export]
    public float _xPanelIn;
    [Export]
    public float _xPanelOut;


    private Acuator _xMove;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _xMove = GetNode<Acuator>("Acuator");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public MoveTask ToPanelIn()
    {
        return _xMove.MoveTo(_xPanelIn);
    }
    public MoveTask ToPanelOut()
    {
        return _xMove.MoveTo(_xPanelOut);
    }

    public PnpArm GetArm()
    {
        return GetNode<PnpArm>("Acuator/PnpArm");
    }

}

using Godot;
using System;

public class OutputPNP : ColorRect
{
    [Export]
    public int _xPanelIn;
    [Export]
    public int _xPanelOut0;
    [Export]
    public int _xPanelOut1;

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

    public MoveTask ToPanelOut(int index)
    {
        if (index == 0)
            return _xMove.MoveTo(_xPanelOut0);
        if (index == 1)
            return _xMove.MoveTo(_xPanelOut1);
        throw new IndexOutOfRangeException();
    }

    public PnpArm GetArm()
    {
        return GetNode<PnpArm>("Acuator/PnpArm");
    }
}

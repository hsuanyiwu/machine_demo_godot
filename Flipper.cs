using Godot;
using System;

public class Flipper : ColorRect
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public ProcessFrame Forward()
    {
        return GetNode<Acuator>("Acuator").MoveTo(180);
    }

    public ProcessFrame Backward()
    {
        return GetNode<Acuator>("Acuator").MoveTo(0);
    }

    public Sucker Sucker()
    {
        return GetNode<Sucker>("Acuator/Platform");
    }
}

using Godot;
using System;

public class Sucker : TextureRect
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    public int SuckTime = 500;

    [Export]
    public int BlowTime = 200;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public ProcessFrame Suck()
    {
        return ProcessFrame.Create((p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    this.Modulate = new Color(88, 0, 0);
                    p.Delay(this.SuckTime);
                    break;
                case 1:
                    p.Exit();
                    break;
            }

        });
    }

    public ProcessFrame Blow()
    {
        return ProcessFrame.Create((p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    this.Modulate = new Color(0, 0, 88);
                    p.Delay(this.BlowTime);
                    break;
                case 1:
                    this.Modulate = this.SelfModulate;
                    p.Exit();
                    break;
            }
        });
    }
}

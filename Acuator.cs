using Godot;
using System;

public enum DIR_TYPE { X, Y }

public class Acuator : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export]
    public DIR_TYPE DirType;
    [Export]
    public bool ReverseDir;
    [Export]
    public float Speed = 50;

    private int _step = -1;
    private float _target;
    private float _pos;

    private float _p0;

    private float _dist;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _p0 = DirType == DIR_TYPE.X ? this.Position.x : this.Position.y;
    }

    public void MoveTo(float target)
    {
        _target = target;
        _dist = target - _pos;
        _step = 10;
    }

    public bool MotionDone
    {
        get { return _step == -1; }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (_step != -1)
        {
            // update move position
            bool done = false;
            if (_dist > 0)
            {
                _dist -= delta * Speed;
                _pos = _target - _dist;
                done = _dist <= 0;
            }
            else
            {
                _dist += delta * Speed;
                _pos = _target - _dist;
                done = _dist >= 0;
            }

            if (done)
            {
                _pos = _target;
                _step = -1;
            }

            // update sprit position
            int sign = ReverseDir ? -1 : 1;
            Vector2 p = this.Position;
            if (DirType == DIR_TYPE.X)
                p.x = _p0 + _pos * sign;
            else
                p.y = _p0 + _pos * sign;
            this.Position = p;
        }
    }
}

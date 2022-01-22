using Godot;
using System;

public enum DIR_TYPE { X, Y, R }

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

    private float _position;
    private int _dir;

    private float _p0;
    private float _r0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _p0 = DirType == DIR_TYPE.X ? this.Position.x : this.Position.y;
        _r0 = this.Rotation;
        _position = 0;
    }

    public bool MotionDone
    {
        get { return _dir == 0; }
    }

    public void SetMotorPosition(float pos)
    {
        _position = pos;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        int sign = ReverseDir ? -1 : 1;
        Vector2 p = this.Position;
        switch (DirType)
        {
            case DIR_TYPE.X:
                p.x = _p0 + _position * sign;
                this.Position = p;
                break;
            case DIR_TYPE.Y:
                p.y = _p0 + _position * sign;
                this.Position = p;
                break;
            case DIR_TYPE.R:
                this.Rotation = _r0 + Mathf.Deg2Rad(_position * sign);
                break;
        }
    }
    public ProcessFrame MoveTo(float target)
    {
        //Console.WriteLine($"target={target}");
        float dist = Math.Abs(target - _position);
        _dir = target > _position ? 1 : -1;

        return ProcessFrame.Create((p) =>
        {
            dist -= ProcessFrameTime.Elapsed * Speed;
            if (dist <= 0)
            {
                dist = 0;
                _dir = 0;
                p.Exit();
            }
            _position = target - dist * _dir;
            //Console.WriteLine($"{_position}");
        });
    }
}

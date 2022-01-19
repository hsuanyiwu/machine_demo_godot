using Godot;
using System;

public class PnpArm : TextureRect
{
    private int _step = -1;
    private Acuator _zMove;
    private int _speed = 5;
    private int _zPosDown = 20;
    private int _zPosUp = 0;
    private int _pos0;
    private float _timeTick;

    private TextureRect _sucker;

    public override void _Ready()
    {
        _zMove = GetNode<Acuator>("Acuator");
        _sucker = GetNode<TextureRect>("Acuator/Sucker");
    }

    public void Pick()
    {
        _step = 100;
    }

    public void Place()
    {
        _step = 200;
    }

    public bool MotionDone
    {
        get { return _step == -1; }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        switch (_step)
        {
            case -1:
                // idle
                break;

            // pick
            case 100:
                // move down
                _zMove.MoveTo(_zPosDown);
                _step += 1;
                break;

            case 101:
                if (_zMove.MotionDone)
                    _step += 1;
                break;

            // suck
            case 102:
                _sucker.Modulate = new Color(255, 0, 0);
                _timeTick = 500;
                _step += 1;
                break;

            case 103:
                _timeTick -= delta * 1000;
                if (_timeTick <= 0)
                    _step += 1;
                break;

            // move up
            case 104:
                _zMove.MoveTo(_zPosUp);
                _step += 1;
                break;

            case 105:
                if (_zMove.MotionDone)
                    _step = -1;
                break;

            // place
            case 200:
                // move down
                _zMove.MoveTo(_zPosDown);
                _step += 1;
                break;

            case 201:
                if (_zMove.MotionDone)
                    _step += 1;
                break;

            // suck
            case 202:
                _sucker.Modulate = _sucker.SelfModulate;
                _timeTick = 500;
                _step += 1;
                break;

            case 203:
                _timeTick -= delta * 1000;
                if (_timeTick <= 0)
                    _step += 1;
                break;

            // move up
            case 204:
                _zMove.MoveTo(_zPosUp);
                _step += 1;
                break;

            case 205:
                if (_zMove.MotionDone)
                    _step = -1;
                break;
        }
    }
}
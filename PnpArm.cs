using Godot;
using System;

public class PnpArm : TextureRect
{
    private Acuator _zMove;
    [Export]
    private int _zPosDown = 20;
    private int _zPosUp = 0;

    private Sucker _sucker;

    public override void _Ready()
    {
        _zMove = GetNode<Acuator>("Acuator");
        _sucker = GetNode<Sucker>("Acuator/Sucker");
    }

    public MoveTask Pick()
    {
        return MoveTask.Create((p) =>
        {
            switch (p.Step)
            {
                case MoveTask.ENTER:
                    p.Wait(_zMove.MoveTo(_zPosDown));
                    break;
                case 1:
                    p.Wait(_sucker.Suck());
                    break;
                case 2:
                    p.Wait(_zMove.MoveTo(_zPosUp));
                    break;
                case 3:
                    p.Exit();
                    break;
            }
        });
    }

    public MoveTask Place()
    {
        return MoveTask.Create((p) =>
        {
            switch (p.Step)
            {
                case MoveTask.ENTER:
                    p.Wait(_zMove.MoveTo(_zPosDown));
                    break;
                case 1:
                    p.Wait(_sucker.Blow());
                    break;
                case 2:
                    p.Wait(_zMove.MoveTo(_zPosUp));
                    break;
                case 3:
                    p.Exit();
                    break;
            }
        });
    }


    public bool MotionDone
    {
        get { return _step == -1; }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
    }

    int _step;
    float _timeTick;

    public void RunLoop_(float delta)
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
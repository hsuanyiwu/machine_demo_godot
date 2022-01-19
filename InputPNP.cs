using Godot;
using System;

public class InputPNP : ColorRect
{
    private Acuator _xMove;
    private PnpArm _pnpArm;

    private int _step = -1;
    public float _xPosPick = 0;
    public float _xPosPlace = 75;
    public float _xPosPaper = 150;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _xMove = GetNode<Acuator>("MoveX");
        _pnpArm = GetNode<PnpArm>("MoveX/PnpArm");
    }

    public PnpArm GetArm()
    {
        return GetNode<PnpArm>("MoveX/PnpArm");
    }

    public void RunLoop()
    {
        _step = 10;
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // run loop
        switch (_step)
        {
            // pick
            case 10:
                _xMove.MoveTo(_xPosPick);
                _step += 1;
                break;

            case 11:
                if (_xMove.MotionDone)
                    _step += 1;
                break;

            case 12:
                _pnpArm.Pick();
                _step += 1;
                break;

            case 13:
                if (_pnpArm.MotionDone)
                    _step = 20;
                break;

            // place
            case 20:
                _xMove.MoveTo(_xPosPlace);
                _step += 1;
                break;

            case 21:
                if (_xMove.MotionDone)
                    _step += 1;
                break;

            case 22:
                _pnpArm.Place();
                _step += 1;
                break;

            case 23:
                if (_pnpArm.MotionDone)
                    _step += 1;
                break;

            case 24:
                _step = 10;
                break;
        }
    }
}

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
        _xMove = GetNode<Acuator>("Acuator");
        _pnpArm = GetNode<PnpArm>("Acuator/PnpArm");
    }

    public PnpArm GetArm()
    {
        return GetNode<PnpArm>("Acuator/PnpArm");
    }

    public MoveTask ToPick()
    {
        return _xMove.MoveTo(_xPosPick);
    }

    public MoveTask ToPlace()
    {
        return _xMove.MoveTo(_xPosPlace);
    }


    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
    }

    private const int IDLE = 0;
    private const int PICK = 100;
    private const int PLACE = 200;

    public void Pick_()
    {
        _step = PICK;
    }

    public bool MotinoDone
    {
        get { return _step == IDLE; }
    }

    private void process_()
    {
        // run loop
        switch (_step)
        {
            // pick
            case PICK:
                _xMove.MoveTo(_xPosPick);
                _step += 1;
                break;

            case PICK + 1:
                if (_xMove.MotionDone)
                    _step += 1;
                break;

            case PICK + 2:
                _pnpArm.Pick();
                _step += 1;
                break;

            case PICK + 3:
                if (_pnpArm.MotionDone)
                    _step = IDLE;
                break;

            // place
            case PLACE:
                _xMove.MoveTo(_xPosPlace);
                _step += 1;
                break;

            case PLACE + 1:
                if (_xMove.MotionDone)
                    _step += 1;
                break;

            case PLACE + 2:
                _pnpArm.Place();
                _step += 1;
                break;

            case PLACE + 3:
                if (_pnpArm.MotionDone)
                    _step = IDLE;
                break;
        }
    }
}

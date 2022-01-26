using Godot;
using System;

public class AutoRun2 : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    private enum Flag
    {
        NONE,
        STOP,
        PUT,
        SAFE,
    }

    private class Status
    {
        public Flag InputPNP;
        public Flag CarA;
        public Flag BackPNP;
        public Flag Flipper;
        public Flag CarB;
        public Flag OutputPNP;
    }

    private Flag _feedType;
    private bool _feedAuto;
    public void FeedPanel(bool feed)
    {
        _feedType = feed ? Flag.PUT : Flag.STOP;
    }

    public void SetAutoFeed(bool enable)
    {
        _feedAuto = enable;
    }

    private ProcessFrame Run_InputPNP(InputPNP inPNP,
        Func<bool> isStop, Func<bool> isPickable, Action setPicked,
        Func<bool> isPlaceable, Action setPlaced, Action setStop)
    {
        return ProcessFrame.Create("Run_InputPNP", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.Wait(inPNP.ToPick());
                    break;
                case 1:
                    if (isPickable())
                        p.Wait(inPNP.GetArm().Pick()).ContinueWith(setPicked);
                    else if (isStop())
                        p.SetStep(10);
                    break;
                case 2:
                    p.Wait(inPNP.ToPlace());
                    break;
                case 3:
                    if (isPlaceable())
                        p.Wait(inPNP.GetArm().Place()).ContinueWith(setPlaced);
                    break;
                case 4:
                    p.SetStep(0);
                    break;

                case 10:
                    if (isPlaceable())
                    {
                        setStop();
                        p.Exit();
                    }
                    break;
            }
        });

    }

    public ProcessFrame StartRun(Machine machine)
    {
        var inPNP = machine.GetNode<InputPNP>("InputPNP");
        var carA = machine.GetNode<Car>("CarA");
        var backPNP = machine.GetNode<BackPNP>("BackPNP");
        var flipper = machine.GetNode<Flipper>("Flipper");
        var carB = machine.GetNode<Car>("CarB");
        var outPNP = machine.GetNode<OutputPNP>("OutputPNP");
        var _record = new Status();
        var rand = new Random();
        _feedType = 0;

        var runAutoFeed = ProcessFrame.Create("Run Auto Feed Panel", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    if (_feedType == Flag.STOP)
                    {
                        _record.InputPNP = Flag.STOP;
                        p.Exit();
                    }
                    else
                    {
                        _record.InputPNP = Flag.PUT;
                        p.StepOne();
                    }
                    break;
                case 1:
                    if (_record.InputPNP == Flag.NONE)
                        p.SetStep(0);
                    break;
            }
        });

        var runFeedPanel = ProcessFrame.Create("Run Feed Panel", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    if (_feedType == Flag.PUT)
                    {
                        _record.InputPNP = Flag.PUT;
                        _feedType = Flag.NONE;
                        p.StepOne();
                    }
                    else if (_feedType == Flag.STOP)
                    {
                        _record.InputPNP = Flag.STOP;
                        p.Exit();
                    }
                    break;
                case 1:
                    if (_record.InputPNP == Flag.NONE)
                        p.SetStep(0);
                    break;
            }
        });

        var runInputPNP = ProcessFrame.Create("Run_InputPNP", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.Wait(inPNP.ToPick());
                    break;
                case 1:
                    if (_record.InputPNP == Flag.PUT)
                        p.Wait(inPNP.GetArm().Pick());
                    else if (_record.InputPNP == Flag.STOP)
                        p.SetStep(10);
                    break;
                case 2:
                    _record.InputPNP = Flag.NONE;
                    p.Wait(inPNP.ToPlace());
                    break;
                case 3:
                    if (_record.CarA == Flag.NONE)
                        p.Wait(inPNP.GetArm().Place());
                    break;
                case 4:
                    _record.CarA = Flag.PUT;
                    p.SetStep(0);
                    break;

                case 10:
                    if (_record.CarA == Flag.NONE)
                    {
                        _record.CarA = Flag.STOP;
                        p.Exit();
                    }
                    break;
            }
        });

        var runInputPNP2 = Run_InputPNP(inPNP,
            isStop: () => { return _record.InputPNP == Flag.STOP; },
            isPickable: () => { return _record.InputPNP == Flag.PUT; },
            setPicked: () => { _record.InputPNP = Flag.NONE; },
            isPlaceable: () => { return _record.CarA == Flag.NONE; },
            setPlaced: () => { _record.CarA = Flag.PUT; },
            setStop: () => { _record.CarA = Flag.STOP; }
        );

        var runCarA = ProcessFrame.Create("Run_CarA", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.Wait(carA.ToPanelIn());
                    break;
                case 1:
                    _record.CarA = Flag.NONE;
                    p.StepOne();
                    break;
                case 2:
                    if (_record.CarA == Flag.PUT)
                        p.Wait(carA.Suck());
                    else if (_record.CarA == Flag.STOP)
                        p.SetStep(10);
                    break;
                case 3:
                    p.Wait(carA.DoScan());
                    break;
                case 4:
                    p.Wait(carA.ToPanelOut());
                    break;
                case 5:
                    p.Wait(carA.Blow());
                    break;
                case 6:
                    _record.BackPNP = Flag.PUT;
                    p.StepOne();
                    break;
                case 7:
                    if (_record.BackPNP == Flag.NONE)
                        p.SetStep(0);
                    break;

                case 10:
                    if (_record.BackPNP == Flag.NONE)
                    {
                        _record.BackPNP = Flag.STOP;
                        p.Exit();
                    }
                    break;
            }
        });

        var runBackPNP = ProcessFrame.Create("Run_BackPNP", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.Wait(backPNP.ToPanelIn());
                    break;
                case 1:
                    if (_record.BackPNP == Flag.PUT)
                        p.Wait(backPNP.GetArm().Pick());
                    else if (_record.BackPNP == Flag.STOP)
                        p.SetStep(10);
                    break;
                case 2:
                    _record.BackPNP = Flag.NONE;
                    p.StepOne();
                    break;
                case 3:
                    if (_record.Flipper == Flag.NONE)
                        p.Wait(backPNP.ToPanelOut());
                    break;
                case 4:
                    p.Wait(backPNP.GetArm().Place());
                    break;
                case 5:
                    _record.Flipper = Flag.PUT;
                    p.Wait(backPNP.ToPanelIn());
                    break;
                case 6:
                    _record.Flipper = Flag.SAFE;
                    p.SetStep(1);
                    break;

                case 10:
                    if (_record.Flipper == Flag.NONE)
                    {
                        _record.Flipper = Flag.STOP;
                        p.Exit();
                    }
                    break;
            }
        });

        var runFlipper = ProcessFrame.Create("Run_Flipper", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    if (_record.Flipper == Flag.PUT)
                        p.Wait(flipper.Sucker().Suck());
                    else if (_record.Flipper == Flag.STOP)
                        p.SetStep(10);
                    break;
                case 1:
                    if (_record.Flipper == Flag.SAFE && _record.CarB == Flag.NONE)
                        p.Wait(flipper.Forward());
                    break;
                case 2:
                    p.Wait(flipper.Sucker().Blow());
                    break;
                case 3:
                    _record.CarB = Flag.PUT;
                    p.Wait(flipper.Backward());
                    break;
                case 4:
                    _record.CarB = Flag.SAFE;
                    _record.Flipper = Flag.NONE;
                    p.SetStep(0);
                    break;

                case 10:
                    if (_record.CarB == Flag.NONE)
                    {
                        _record.CarB = Flag.STOP;
                        p.Exit();
                    }
                    break;
            }
        });

        var runCarB = ProcessFrame.Create("Run_CarB", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.Wait(carB.ToPanelIn());
                    break;
                case 1:
                    _record.CarB = Flag.NONE;
                    p.StepOne();
                    break;
                case 2:
                    if (_record.CarB == Flag.PUT)
                        p.Wait(carB.Suck());
                    else if (_record.CarB == Flag.STOP)
                        p.SetStep(10);
                    break;
                case 3:
                    if (_record.CarB == Flag.SAFE)
                        p.Wait(carB.DoScan());
                    break;
                case 4:
                    p.Wait(carB.ToPanelOut());
                    break;
                case 5:
                    p.Wait(carB.Blow());
                    break;
                case 6:
                    _record.OutputPNP = Flag.PUT;
                    p.StepOne();
                    break;
                case 7:
                    if (_record.OutputPNP == Flag.NONE)
                        p.SetStep(0);
                    break;

                case 10:
                    if (_record.OutputPNP == Flag.NONE)
                        p.Wait(carB.ToPanelOut());
                    break;
                case 11:
                    _record.OutputPNP = Flag.STOP;
                    p.Exit();
                    break;
            }
        });

        var runOutputPNP = ProcessFrame.Create("Run_OutputPNP", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.Wait(outPNP.ToPanelIn());
                    break;
                case 1:
                    if (_record.OutputPNP == Flag.PUT)
                        p.Wait(outPNP.GetArm().Pick());
                    else if (_record.OutputPNP == Flag.STOP)
                        p.SetStep(10);
                    break;
                case 2:
                    _record.OutputPNP = Flag.NONE;
                    p.Wait(outPNP.ToPanelOut(rand.Next(0, 1)));
                    break;
                case 3:
                    p.Wait(outPNP.GetArm().Place());
                    break;
                case 4:
                    p.SetStep(0);
                    break;

                case 10:
                    p.Exit();
                    break;
            }
        });

        return ProcessFrame.Create((p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    if (_feedAuto)
                        p.Wait(runAutoFeed);
                    else
                        p.Wait(runFeedPanel);
                    p.Wait(runInputPNP);
                    p.Wait(runCarA);
                    p.Wait(runBackPNP);
                    p.Wait(runFlipper);
                    p.Wait(runCarB);
                    p.Wait(runOutputPNP);
                    break;
                case 1:
                    p.Exit();
                    break;
            }
        });
    }
}

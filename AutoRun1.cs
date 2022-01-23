using Godot;
using System;

public class AutoRun1 : Node
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

    public ProcessFrame StartRun(Machine machine)
    {
        var inPNP = machine.InputPNP();
        var carA = machine.CarA();
        var backPNP = machine.GetNode<BackPNP>("BackPNP");
        var flipper = machine.GetNode<Flipper>("Flipper");
        var carB = machine.GetNode<Car>("CarB");
        var outPNP = machine.GetNode<OutputPNP>("OutputPNP");

        var runInputPNP = ProcessFrame.Create("Run_InputPNP", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.aWait(inPNP.ToPick());
                    break;
                case 1:
                    p.aWait(inPNP.GetArm().Pick());
                    break;
                case 2:
                    p.aWait(inPNP.ToPlace());
                    break;
                case 3:
                    p.aWait(inPNP.GetArm().Place());
                    break;
                case 4:
                    p.SetStep(0);
                    break;
            }
        });

        var runCarA = ProcessFrame.Create("Run_CarA", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.aWait(carA.ToPanelIn());
                    break;
                case 1:
                    p.aWait(carA.DoScan());
                    break;
                case 2:
                    p.aWait(carA.ToPanelOut());
                    break;
                case 3:
                    p.SetStep(0);
                    break;
            }
        });

        var runBackPNP = ProcessFrame.Create("Run_BackPNP", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.aWait(backPNP.ToPanelIn());
                    break;
                case 1:
                    p.aWait(backPNP.GetArm().Pick());
                    break;
                case 2:
                    p.aWait(backPNP.ToPanelOut());
                    break;
                case 3:
                    p.aWait(backPNP.GetArm().Pick());
                    break;
                case 4:
                    p.SetStep(0);
                    break;
            }
        });

        var runFlipper = ProcessFrame.Create("Run_Flipper", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.aWait(flipper.Sucker().Suck());
                    break;
                case 1:
                    p.aWait(flipper.Forward());
                    break;
                case 2:
                    p.aWait(flipper.Sucker().Blow());
                    break;
                case 3:
                    p.aWait(flipper.Backward());
                    break;
                case 4:
                    p.Exit();//SetStep(0);
                    break;
            }
        });

        var runCarB = ProcessFrame.Create("Run_CarB", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.aWait(carB.ToPanelIn());
                    break;
                case 1:
                    p.aWait(carB.DoScan());
                    break;
                case 2:
                    p.aWait(carB.ToPanelOut());
                    break;
                case 3:
                    p.SetStep(0);
                    break;
            }
        });

        var runOutputPNP = ProcessFrame.Create("Run_OutputPNP", (p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.aWait(outPNP.ToPanelIn());
                    break;
                case 1:
                    //if (_count.)
                    p.aWait(outPNP.GetArm().Pick());
                    break;
                case 2:
                    //_count.
                    p.aWait(outPNP.ToPanelOut(0));
                    break;
                case 3:
                    p.aWait(outPNP.GetArm().Place());
                    break;
                case 4:
                    p.SetStep(0);
                    break;
            }
        });

        return ProcessFrame.Create((p) =>
        {
            switch (p.Step)
            {
                case ProcessFrame.ENTER:
                    p.aWait(runInputPNP);
                    p.aWait(runCarA);
                    p.aWait(runBackPNP);
                    p.aWait(runFlipper);
                    p.aWait(runCarB);
                    p.aWait(runOutputPNP);
                    break;
                case 1:
                    p.Exit();
                    break;
            }
        });
    }
}

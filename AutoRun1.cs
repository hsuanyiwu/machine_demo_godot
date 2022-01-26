using Godot;
using System;
using System.Threading.Tasks;

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

    private bool _stop = false;
    public void StopTest()
    {
        _stop = true;
    }

    public ProcessFrame StartTest(Machine machine)
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
                    p.Wait(inPNP.ToPick());
                    break;
                case 1:
                    p.Wait(inPNP.GetArm().Pick());
                    break;
                case 2:
                    p.Wait(inPNP.ToPlace());
                    break;
                case 3:
                    p.Wait(inPNP.GetArm().Place());
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
                    p.Wait(carA.ToPanelIn());
                    break;
                case 1:
                    p.Wait(carA.DoScan());
                    break;
                case 2:
                    p.Wait(carA.ToPanelOut());
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
                    p.Wait(backPNP.ToPanelIn());
                    break;
                case 1:
                    p.Wait(backPNP.GetArm().Pick());
                    break;
                case 2:
                    p.Wait(backPNP.ToPanelOut());
                    break;
                case 3:
                    p.Wait(backPNP.GetArm().Pick());
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
                    p.Wait(flipper.Sucker().Suck());
                    break;
                case 1:
                    p.Wait(flipper.Forward());
                    break;
                case 2:
                    p.Wait(flipper.Sucker().Blow());
                    break;
                case 3:
                    p.Wait(flipper.Backward());
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
                    p.Wait(carB.ToPanelIn());
                    break;
                case 1:
                    p.Wait(carB.DoScan());
                    break;
                case 2:
                    p.Wait(carB.ToPanelOut());
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
                    p.Wait(outPNP.ToPanelIn());
                    break;
                case 1:
                    p.Wait(outPNP.GetArm().Pick());
                    break;
                case 2:
                    p.Wait(outPNP.ToPanelOut(0));
                    break;
                case 3:
                    p.Wait(outPNP.GetArm().Place());
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

    /*public async Task RunTest_Async()
    {
        var runInput = Task.Run(async () =>
        {
            await inputPNP.ToPanelIn();
            await inputPNP.DoPick();
            await inputPNP.ToPanelOut();
            await InputPNP.DoPlace();
        });

        var runCarA = Task.Run(async () =>
        {
            await carA.ToPanelIn();
            await carA.DoScan();
            await carA.ToPanelOut();
        });

        //...

        Task.WaitAll(...);
    }*/
}

using System;
using System.Collections.Generic;
using System.Text;

public class ProcessFrameTime
{
    public static float Elapsed = 1.0f;
}

public class ProcessFrame
{
    private ProcessFrame _parent;
    private Action<ProcessFrame> _function;
    private List<ProcessFrame> _subFrames = new List<ProcessFrame>();
    private string _name = string.Empty;
    private int _step;
    private Action _after;

    public const int ENTER = 0;

    private ProcessFrame()
        : this(null)
    {
    }

    private ProcessFrame(Action<ProcessFrame> func)
    {
        if (func != null)
            _name = func.Method.Name;
        _function = func;
        _step = ENTER;
    }

    private ProcessFrame Add(ProcessFrame subFrame)
    {
        if (subFrame._parent != null)
            subFrame._parent._subFrames.Remove(subFrame);
        subFrame._parent = this;
        _subFrames.Add(subFrame);
        return subFrame;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public ProcessFrame Wait(ProcessFrame subFrame)
    {
        return Add(subFrame);
    }

    public ProcessFrame aWait(Action<ProcessFrame> actFrame)
    {
        return Add(new ProcessFrame(actFrame));
    }

    public int Step
    {
        get { return _step; }
    }

    public void SetStep(int step)
    {
        _step = step;
    }

    public void StepOne()
    {
        _step++;
    }

    public void Exit()
    {
        _subFrames.Clear();
        _function = null;
    }

    public bool IsExit()
    {
        return _subFrames.Count == 0 && _function == null;
    }

    public void ContinueWith(Action func)
    {
        _after = func;
    }

    private void Update()
    {
        if (_subFrames.Count > 0)
        {
            for (int i = _subFrames.Count - 1; i >= 0; --i)
            {
                _subFrames[i].Update();
                if (_subFrames[i].IsExit())
                    _subFrames.RemoveAt(i);
            }
            if (_subFrames.Count != 0)
                return;
            _step++;
        }

        if (_function != null)
        {
            _function(this);
            if (_function == null && _after != null)
                Defer(_after);
        }
    }

    private void Print(StringBuilder sb, string prefix, bool tail)
    {
        if (_function != null)
        {
            sb.Append(prefix + (tail ? "L" : "|-"));
            sb.Append($"< {_name} > Step: {_step}");
            sb.AppendLine();
        }

        if (_subFrames.Count > 0)
        {
            prefix += (tail ? "     " : "|    ");
            int i = 0;
            for (; i < _subFrames.Count - 1; ++i)
                _subFrames[i].Print(sb, prefix, false);
            _subFrames[i].Print(sb, prefix, true);
        }
    }

    private static ProcessFrame _root = new ProcessFrame();
    private static List<Action> _defer = new List<Action>();

    public static ProcessFrame Create(Action<ProcessFrame> function)
    {
        return new ProcessFrame(function);
    }

    /*public static ProcessFrame WaitAll(params ProcessFrame[] frames)
    {
        var tmp = new ProcessFrame(null);
        foreach (var f in frames)
            tmp.Add(f);
        return tmp;
    }*/

    public static ProcessFrame Create(string name, Action<ProcessFrame> function)
    {
        var frame = new ProcessFrame(function);
        frame.SetName(name);
        return frame;
    }

    public static ProcessFrame Emit(ProcessFrame process)
    {
        _root.Add(process);
        return process;
    }

    public static void Terminate()
    {
        _root.Exit();
    }

    public static void Defer(Action act)
    {
        _defer.Add(act);
    }

    public static void MoveStep()
    {
        _root.Update();
        // sync-lock ?
        for (int i = _defer.Count - 1; i >= 0; --i)
        {
            _defer[i]();
            _defer.RemoveAt(i);
        }
    }

    public static string CallStack()
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        _root.Print(sb, "", true);
        sb.AppendLine("}");
        return sb.ToString();
    }

}
public static class ProcessFrameExt
{
    public static void Delay(this ProcessFrame root, int msec)
    {
        float tick = msec;
        root.Wait(ProcessFrame.Create("Delay", (p) =>
        {
            if ((tick -= ProcessFrameTime.Elapsed * 1000) <= 0)
                p.Exit();
        }));
    }
}
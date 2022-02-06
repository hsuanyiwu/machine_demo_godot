using System;
using System.Collections.Generic;
using System.Text;

public class MoveTaskTime
{
    public static float Elapsed = 1.0f;
}

public class MoveTask
{
    private MoveTask _parent;
    private Action<MoveTask> _function;
    private List<MoveTask> _subFrames = new List<MoveTask>();
    private string _name = string.Empty;
    private int _step;
    private Action _after;

    public const int ENTER = 0;

    private MoveTask()
        : this(null)
    {
    }

    private MoveTask(Action<MoveTask> func)
    {
        if (func != null)
            _name = func.Method.Name;
        _function = func;
        _step = ENTER;
    }

    private MoveTask Add(MoveTask subFrame)
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

    public MoveTask Wait(MoveTask subFrame)
    {
        return Add(subFrame);
    }

    public MoveTask aWait(Action<MoveTask> actFrame)
    {
        return Add(new MoveTask(actFrame));
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

    private static MoveTask _root = new MoveTask();
    private static List<Action> _defer = new List<Action>();

    public static MoveTask Create(Action<MoveTask> function)
    {
        return new MoveTask(function);
    }

    /*public static ProcessFrame WaitAll(params ProcessFrame[] frames)
    {
        var tmp = new ProcessFrame(null);
        foreach (var f in frames)
            tmp.Add(f);
        return tmp;
    }*/

    public static MoveTask Create(string name, Action<MoveTask> function)
    {
        var frame = new MoveTask(function);
        frame.SetName(name);
        return frame;
    }

    public static MoveTask Emit(MoveTask process)
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
public static class MoveTaskExt
{
    public static void Delay(this MoveTask root, int msec)
    {
        float tick = msec;
        root.Wait(MoveTask.Create("Delay", (p) =>
        {
            if ((tick -= MoveTaskTime.Elapsed * 1000) <= 0)
                p.Exit();
        }));
    }
}
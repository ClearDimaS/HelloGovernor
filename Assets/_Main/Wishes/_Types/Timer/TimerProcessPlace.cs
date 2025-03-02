using System;

public class TimerProcessPlace : ProcessPlace
{
    public override bool CanAddProgress(CitizenController citizen)
    {
        return true;
    }
}
using System;

public class TargetDespawnedException : Exception
{

    public TargetDespawnedException()
    {
    }

    public TargetDespawnedException(string message)
        : base(message)
    {
    }

    public TargetDespawnedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
using UnityEngine;

public static class InputGate
{
    public static bool CanAcceptInput { get; private set; } = true;

    public static void BlockInput() => CanAcceptInput = false;
    public static void AllowInput() => CanAcceptInput = true;
}

using UnityEngine;

public static class LayerUtils
{
    public static int Player = LayerMask.NameToLayer("Player");
    public static int Enemy = LayerMask.NameToLayer("Enemy");
    public static int Ground = LayerMask.NameToLayer("Ground");
    public static int Attackable = LayerMask.NameToLayer("Attackable");
    public static int Portal = LayerMask.NameToLayer("Portal");
}
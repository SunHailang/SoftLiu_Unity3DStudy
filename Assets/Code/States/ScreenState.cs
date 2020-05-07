using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoftLiu.States;

public class ScreenState : State
{
    public enum EntryBehaviour
    {
        None = 0,
        Load = 1,
        LoadAdditive = 2
    }
}

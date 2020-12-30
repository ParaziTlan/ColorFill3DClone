using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LevelScriptableObject : ScriptableObject
{
    public SerializableState2DArrayClass State2DArray;
    public int Width;
    public int Height;
    public SerializedEnemyVariables[] EnemyArray;
}

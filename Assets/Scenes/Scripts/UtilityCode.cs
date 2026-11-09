using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityCode 
{
    public static GameObject GetGameObjInResource(string Path)
    {
        return Resources.Load<GameObject>(Path);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskUtils
{
    public static async Task WaitUntil(Func<bool> predicate, int sleep = 50)
    {
        while (!predicate() && true)
        {
            await Task.Delay(sleep);
        }
    }
}

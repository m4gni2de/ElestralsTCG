using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClickHelpers
{
    
    public static string WhoAmI(this object obj)
    {
        return App.Account.Id;
    }
}

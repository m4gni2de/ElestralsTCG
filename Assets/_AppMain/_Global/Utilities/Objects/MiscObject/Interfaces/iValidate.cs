using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iValidate
{
    List<string> ErrorList { get; }
    void AddError(string msg);
    bool Validate();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidationObject : MonoBehaviour, iValidate
{
    private List<string> _errorList = null;
    public List<string> ErrorList
    {
        get
        {
            _errorList ??= new List<string>();
            return _errorList;
        }
    }

    public virtual void AddError(string msg)
    {
        ErrorList.Add(msg);
    }

    public virtual bool Validate()
    {
        return true;
    }
}

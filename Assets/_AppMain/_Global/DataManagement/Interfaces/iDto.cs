using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDto<T>
{
    T ZeroDTO { get; }
    void SetZeroDTO(T dto);
    T GetDTO { get; }
}


public static class DtoExtensions
{
    public static bool IsDirty<T>(this iDto<T> dto)
    {
        T zero = dto.ZeroDTO;
        T current = dto.GetDTO;

        foreach (var item in zero.GetType().GetProperties())
        {
            object zVal = item.GetValue(zero);
            object currVal = item.GetValue(current);

            if (zVal.CompareTo(currVal) != ComparedTo.EqualTo) { return true; }
        }
        return false;
    }
}

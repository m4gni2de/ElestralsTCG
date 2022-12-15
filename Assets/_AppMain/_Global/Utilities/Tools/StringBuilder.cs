using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalUtilities
{
    public static class StringBuilder 
    {
        public static string InAlphabet(this int index)
        {
            index = Mathf.Clamp(index, 0, 25);

            List<string> letters = new List<string>();
            string alpha = "abcdefghijklmnopqrstuvwxyz";

            for (int i = 0; i < alpha.Length; i++)
            {
                letters.Add(alpha[i].ToString());
            }

            return letters[index];
        }
        //public static string FromList(List<string> items, bool canBePlural)
        //{
           
        //    if (canBePlural) { return FromListPlural(items); }
        //    string msg = "";

        //    if (items.Count == 1)
        //    {
        //       msg = $""
        //    }
        //    for (int i = 0; i < items.Count; i++)
        //    {
               
        //        msg += $""
        //    }
        //}

        //private static string FromListPlural(List<string> items)
        //{

        //}

            
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalUtilities;
using UnityEngine;
public class UniqueString
{
    private string _value = "";
    public string value { get { return _value; } }
    public static implicit operator string(UniqueString a)
    {
        return a.value;
    }


    private static int minStringLength = 8;

    UniqueString(string uniqueValue)
    {
        _value = uniqueValue;
    }
    public UniqueString(string prefix = "", int maxLength = -1)
    {
        bool hasMax = maxLength > -1;

        if (hasMax)
        {
            if (maxLength >= 0 && maxLength < minStringLength)
            {
                maxLength = minStringLength;
            }

        }
        _value = Create(prefix, maxLength);
    }

    public static UniqueString GetShortId(string prefix = "", int length = 2)
    {
        string tempId = CreateId(length, prefix);
        return new UniqueString(tempId);
    }
    public static UniqueString WithSetLength(string prefix, int length)
    {
        string str = Create(prefix, length);
        return new UniqueString(str);
    }
    public static string Create(string prefix = "", int maxLength = -1)
    {

        List<string> letters = new List<string>();
        string alpha = "abcdefghijklmnopqrstuvwxyz";

        for (int i = 0; i < alpha.Length; i++)
        {
            letters.Add(alpha[i].ToString());
        }


        if (prefix.Length > 3)
        {
            prefix = prefix.Substring(0, 3);
        }
        string key = "";
        DateTime now = DateTime.Now;
        string year = now.Year.ToString();
        string ms = DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.DayOfYear.ToString() + DateTime.Now.Hour + DateTime.Now.Minute;

        key = ms + year.Substring(2);


        string newKey = "";
        for (int i = 0; i < key.Length; i++)
        {
            string ch = key[i].ToString();

            int rand = UnityEngine.Random.Range(0, 2);
            if (rand < 1)
            {

                int rand2 = UnityEngine.Random.Range(0, letters.Count);
                string randChar = letters[rand2].ToUpper();
                ch = randChar;
            }

            newKey += ch;
        }

        key = prefix + newKey;

        if (maxLength > 0 && key.Length > maxLength)
        {
            key = key.Substring(0, maxLength);
        }
        return key;
    }

    public static string CreateId(int digits, string prefix = "")
    {
        string id = prefix;
        for (int i = 0; i < digits; i++)
        {
            int digit = UnityEngine.Random.Range(0, 10);
            id += digit.ToString();
        }

        return id;
    }

    public static string RandomLetters(int count)
    {
        string letters = "";
        for (int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(0, 26);
            letters += rand.InAlphabet();
        }
        return letters;
    }
}

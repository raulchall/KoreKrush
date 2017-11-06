using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Helpers
{
    public static void Shuffle<T>(this T[] array)
    {
        for (int i = 0; i < array.Length; i++) {
            int pos = Random.Range (i, array.Length);

            var tmp = array [i];
            array [i] = array [pos];
            array [pos] = tmp;
        }
    }

    public static T Choice<T>(this T[] array)
    {
        return array [Random.Range (0, array.Length)];
    }

    public static T Last<T>(this List<T> list)
    {
        return list [list.Count - 1];
    }

}

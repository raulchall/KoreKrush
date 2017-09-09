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

	public static float Multiplier(int count)
	{
		if (count < 7)
			return 1;
		else if (count < 11)
			return 1.25f;
		else if (count < 16)
			return 1.5f;
		else
			return 2;
	}

	public static float VirtualSpeedToPathSpeed(float virtualSpeed)
	{
		float a1 = 0.09f / 2950;
		float a0 = 0.01f - 50 * a1;
		return a0 + virtualSpeed*a1;
	}

	public static float VirtualDistanceToPathDistance(float virtualDistance, float pathSize, float virtualPathSize)
	{
		return (virtualDistance * pathSize) / virtualPathSize;
	}
}

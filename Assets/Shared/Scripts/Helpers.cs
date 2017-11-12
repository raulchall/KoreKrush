using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

	public static void Push<T>(this LinkedList<T> list, T value)
	{
		list.AddLast(value);
	}

	public static T Pop<T>(this LinkedList<T> list) where T : class
	{
		if (list.Count == 0) return null;

		var l = list.Last.Value;
		list.RemoveLast();
		return l;
	}
	
	public static T Peek<T>(this LinkedList<T> list) where T : class
	{
		return list.Count == 0 ? null : list.Last.Value;
	}

	public static void SetAs(this Transform transform, Transform source, 
		bool setPosition = true, bool setRotation = true, bool setScale = true, bool setTweens = true)
	{
		if (setPosition) transform.position = source.position;
		if (setRotation) transform.rotation = source.rotation;
		if (setScale) transform.localScale = source.localScale;
		if (!setTweens) return;
		var tweens = DOTween.TweensByTarget(source);
		if (tweens != null) tweens.ForEach(t => t.SetTarget(transform));
	}
}

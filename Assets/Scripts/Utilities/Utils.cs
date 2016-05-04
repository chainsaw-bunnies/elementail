using UnityEngine;

public static class Utils
{
  public static T RandomElement<T>(this T[] array)
  {
    return array[Random.Range(0, array.Length)];
  }
}

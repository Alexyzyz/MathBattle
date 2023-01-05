using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilMath
{

  /// <summary>
  /// Takes in an initial value and its delta and returns the value it would have been after wrapping around the min and max parameters.
  /// </summary>
  public static int WrapValue(int value, int delta, int min, int max) {
    int newValue = value + delta;
    if (newValue < min) {
      return max - 1 + Mathf.Abs(newValue % max);
    }
    if (newValue > max) {
      return newValue % max - 1;
    }
    return newValue;
  }

}

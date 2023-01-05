using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilColor
{

  public static Color SetAlpha(Color input, float alpha) {
    Color output = input;
    output.a = alpha;
    return output;
  }

  public static Color Black = Color.black;
  public static Color White = Color.white;

  public static Color Cyan = new Color32(22, 210, 196, 255);

  public static Color TileHero = new Color32(50, 105, 25, 255);
  public static Color TileEnemy = new Color32(110, 30, 25, 255);

  public static Color HP = new Color32(245, 154, 149, 255);
  public static Color AP = new Color32(211, 255, 0, 255);
  public static Color MP = new Color32(255, 64, 184, 255);

}

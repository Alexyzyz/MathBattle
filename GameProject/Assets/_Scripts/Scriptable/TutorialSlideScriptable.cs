using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Tutorial Slide")]
public class TutorialSlideScriptable : ScriptableObject
{

  public string title;
  public Sprite image;

  [Space]

  [TextArea(maxLines: 5, minLines: 3)] public string description;

}

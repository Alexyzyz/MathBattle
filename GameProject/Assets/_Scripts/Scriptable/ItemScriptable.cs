using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item")]
public class ItemScriptable : ScriptableObject
{

  public string idName;
  public string title;

  [TextArea(maxLines: 5, minLines: 1)] public string description;

  [Space]

  [TextArea(maxLines: 5, minLines: 1)] public string flavorText;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Object/Skill")]
public class SkillScriptable : ScriptableObject
{

  public string id;
  public string title;
  [TextArea(6, 6)] public string description;
  
  [Space]

  public int APCost;
  public int MPCost;

  [Space]

  public float successTime;
  public float critTime;
  public float critBonus;

  [HideInInspector] public bool damageIsRandom;
  [HideInInspector] public int damage;
  [HideInInspector] public int damageMin;
  [HideInInspector] public int damageMax;

}

#if UNITY_EDITOR
[CustomEditor(typeof(SkillScriptable))]
public class SkillScriptableEditor : Editor
{
  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    SkillScriptable script = (SkillScriptable)target;

    EditorGUILayout.Space();

    script.damageIsRandom = EditorGUILayout.Toggle("Damage is random", script.damageIsRandom);
    if (script.damageIsRandom)
    {
      script.damageMin = EditorGUILayout.IntField("Min damage", script.damageMin);
      script.damageMax = EditorGUILayout.IntField("Max damage", script.damageMax);
    } else {
      script.damage = EditorGUILayout.IntField("Damage", script.damage);
    }

  }
}
#endif
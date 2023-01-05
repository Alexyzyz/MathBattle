using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Object/Unit")]
public class BaseUnitScriptable : ScriptableObject
{

  public BattleUnitBase prefab;

  public string title;
  [TextArea] public string description;

  public UnitStats baseStats;

  public List<SkillScriptable> startingSkills;
  public List<Item> startingItems;

}

[Serializable]
public class UnitStats
{
  public int maxHP;
  public int maxAP;
  public int maxMP;

  public int HP;
  public int AP;
  public int MP;

  public UnitStats(int maxHP, int maxAP, int maxMP) {
    this.maxHP = maxHP;
    this.maxAP = maxAP;
    this.maxMP = maxMP;

    HP = maxHP;
    AP = maxAP;
    MP = maxMP;
  }
}

[Serializable]
public enum UnitFaction
{
  Hero,
  Enemy
}
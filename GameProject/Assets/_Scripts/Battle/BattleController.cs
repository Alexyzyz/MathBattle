using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData
{
  public BattleUnitBase attacker;
  public List<BattleUnitBase> targetList;
  public SkillScriptable attackerSkill;

  public bool isSuccessful;
  public bool isCrit;
}

public class MathData
{
  public MathOperation operation;
  public int leftValue, rightValue, correctAnswer;
  public float graceTime, maxTime, critTime;

  public int damageMin, damageMax;
  public float critBonus;
}

public class DifficultyData
{
  // for "easy" operations, eg additions and subtractions
  public int easyMinValue;
  public int easyMaxValue;

  // for "hard" operations, eg multiplication and division
  public int hardMinValue;
  public int hardMaxValue;

  // enemy difficulty
  public int minEnemyCount;
  public int enemyMaxHP;
  public int enemyAttackDamage;
  public float enemyAttackAnswerTime;
}

public enum MathOperation
{
  Add, Sub, Mul, Div
}

public class BattleController : Singleton<BattleController>
{

  public AttackData AttackData { get; private set; } = new AttackData {
    attacker = null,
    targetList = new List<BattleUnitBase>(),
    attackerSkill = null,
  };

  public MathData MathData { get; private set; } = new MathData {
    operation = MathOperation.Add,
    leftValue = 1,
    rightValue = 1,
    correctAnswer = 2,

    graceTime = 1,
    maxTime = 1,
    critTime = 1,

    damageMin = 1,
    damageMax = 1,
    critBonus = 1,
  };

  public DifficultyData DiffData { get; private set; } = new DifficultyData {
    easyMaxValue = 1,
    easyMinValue = 1,

    hardMaxValue = 1,
    hardMinValue = 1,

    minEnemyCount = 1,
    enemyMaxHP = 30,
    enemyAttackDamage = 50,
    enemyAttackAnswerTime = 3,
  };

  public BattleRecord BattleRecord = new BattleRecord {
    datetime = System.DateTime.Now,
    turnCount = 1,
    defeatCount = 0,

    totalAnswers = 0,
    correctAnswers = 0,
    avgAnswerTime = 0,
  };

  public List<ItemScriptable> StealableItemList;
  public bool IsEnemyTurn { get; set; }

  private float totalAnswerTime;

  // public methods

  public void HandleEnemyDefeat() {
    DefeatCount++;
    BattleCanvasMenuBattleInfo.Instance.SetDefeatedCount(DefeatCount);

    if (BattleUnitManager.Instance.EnemyUnitList.Count == 0) {
      // if all enemies have been defeated during the player's turn
      BattleCanvasInfoBanner.Instance.Show("Lawan baru bermunculan!", 3);
      BattleUnitManager.Instance.SpawnNewEnemies();
    }
  }

  public void HandleCorrectAnswer(float answerTime) {
    GameSoundManager.Instance.PlaySound(GameSound.BattleAnswerCorrect);

    CorrectAnswers++;

    totalAnswerTime += answerTime;
    AvgAnswerTime = totalAnswerTime / CorrectAnswers;

    AttackData.isSuccessful = true;
    HandleNewAnswer();
  }

  public void HandleIncorrectAnswer() {
    GameSoundManager.Instance.PlaySound(GameSound.BattleAnswerIncorrect);
    HandleNewAnswer();
  }

  public void SetMathData(SkillScriptable skill = null) {

    // decide the operation
    MathOperation op = GetOperation();
    int leftVal, rightVal, correctAnswer;

    if (op == MathOperation.Add || op == MathOperation.Sub) {
      // make an "easy" question
      leftVal = Random.Range(DiffData.easyMinValue, DiffData.easyMaxValue + 1);
      rightVal = Random.Range(DiffData.easyMinValue, DiffData.easyMaxValue + 1);

      if (op == MathOperation.Add) {
        correctAnswer = leftVal + rightVal;
      } else {
        if (rightVal > leftVal) {
          // avoid negative correct answers
          int temp = leftVal;
          leftVal = rightVal;
          rightVal = temp;
        }
        correctAnswer = leftVal - rightVal;
      }
    } else {
      // make a "hard" question
      leftVal = Random.Range(DiffData.hardMinValue, DiffData.hardMaxValue + 1);
      rightVal = Random.Range(DiffData.hardMinValue, DiffData.hardMaxValue + 1);

      int multiplied = leftVal * rightVal;
      if (op == MathOperation.Mul) {
        correctAnswer = multiplied;
      }
      else {
        correctAnswer = leftVal;
        leftVal = multiplied;
      }
    }

    // calc outgoing damage
    int damageMin, damageMax;

    if (skill != null) {
      // refer to player skill
      damageMin = damageMax = skill.damage;

      if (skill.damageMax > 0) {
        // zero max damage is reserved for signifying non-random damage value
        damageMin = skill.damageMin;
        damageMax = skill.damageMax;
      }
    }
    else {
      // refer to enemy difficulty data
      damageMin = damageMax = DiffData.enemyAttackDamage;
    }

    MathData = new MathData {
      operation = op,
      leftValue = leftVal,
      rightValue = rightVal,
      correctAnswer = correctAnswer,

      graceTime = 1.4f,
      maxTime = skill ? skill.successTime : DiffData.enemyAttackAnswerTime,
      critTime = skill ? skill.critTime : 0.5f * DiffData.enemyAttackAnswerTime,

      damageMin = damageMin,
      damageMax = damageMax,
      critBonus = skill ? skill.critBonus : 1,
    };

    MathOperation GetOperation() {
      if (Turn == 1) return MathOperation.Add;
      if (Turn == 2) return MathOperation.Sub;
      if (2 < Turn && Turn <= 5) return UtilGlobal.GetRandomItem(new List<MathOperation> { MathOperation.Add, MathOperation.Sub });
      if (Turn == 6) return MathOperation.Mul;
      if (Turn == 7) return MathOperation.Div;
      if (7 < Turn && Turn <= 9) return UtilGlobal.GetRandomItem(new List<MathOperation> { MathOperation.Mul, MathOperation.Div });

      return UtilGlobal.GetRandomItem(new List<MathOperation> { MathOperation.Add, MathOperation.Div, MathOperation.Mul, MathOperation.Sub });
    }
  }

  public void HandleEndAttack() {
    if (BattleUnitManager.Instance.HeroUnitList.Count == 0) {
      // end the game after this attack
      BattleStateManager.Instance.SwitchState(BattleState.GameOver);
      return;
    }

    if (IsEnemyTurn) {
      // it's still the enemy's turn, continue
      BattleStateEnemyTurnParam param = new BattleStateEnemyTurnParam { isContinuation = true };
      BattleStateManager.Instance.SwitchState(BattleState.EnemyTurn, param);
      return;
    }

    // clear attack data

    AttackData.attacker = null;
    AttackData.targetList.Clear();
    AttackData.attackerSkill = null;

    BattleStateManager.Instance.SwitchState(BattleState.MainMenu);
  }

  public void EndTurn() {
    IsEnemyTurn = false;
    RefreshUnitAP();

    // spawn new enemies maybe

    if (BattleUnitManager.Instance.EnemyUnitList.Count < DiffData.minEnemyCount) {
      BattleCanvasInfoBanner.Instance.Show("Saatnya giliran kamu!  Lawan baru bermunculan...", 3);
      BattleUnitManager.Instance.SpawnNewEnemies();
    } else {
      BattleCanvasInfoBanner.Instance.Show("Saatnya giliran kamu!", 2);
    }

    // update difficulty values

    BattleCanvasMenuBattleInfo.Instance.SetTurn(++Turn);
    UpdateDifficulty();

    // go back to main menu

    BattleStateManager.Instance.SwitchState(BattleState.MainMenu);
  }

  private void RefreshUnitAP() {
    foreach (BattleUnitBase unit in BattleUnitManager.Instance.UnitList) {
      unit.Stats.maxAP = Mathf.Min(unit.Stats.maxAP + 1, 30);
      unit.AP = unit.Stats.maxAP;
    }
  }

  private void UpdateDifficulty() {
    DiffData.minEnemyCount = (int)Mathf.Min(5, Mathf.Ceil(3 * Mathf.Log(Turn, 10) + 1));

    DiffData.enemyMaxHP = (int)Mathf.Ceil(40 * Mathf.Log(1.75f * Turn, 10) + 20);

    DiffData.enemyAttackDamage = (int)(30 * Mathf.Log(Turn, 10) + 10);
    DiffData.enemyAttackAnswerTime = Turn <= 20 ? 3 : 3 - 0.9f * Mathf.Log(Turn - 19, 10);

    int easyMaxValue = EasyMaxValue(Turn);
    DiffData.easyMaxValue = Mathf.Min(easyMaxValue, 500);
    DiffData.easyMinValue = Mathf.Max(easyMaxValue / 2, 2);

    int hardMaxValue = HardMaxValue(Turn);
    DiffData.hardMaxValue = Mathf.Min(hardMaxValue, 100);
    DiffData.hardMinValue = Mathf.Max(hardMaxValue / 2, 2);

    int EasyMaxValue(int turn) {
      const int TAPER_POINT = 20;
      return turn < TAPER_POINT ? Return(Piece1(turn)) : Return(Piece2(turn));

      float Piece1(float i) => Mathf.Pow(i, 1.5f) + 3;
      float Piece2(float i) {
        return Sub(i) + Piece1(TAPER_POINT) - Sub(TAPER_POINT);
        float Sub(float i) => 50 * Mathf.Log(i, 10);
      }
      int Return(float f) => (int)Mathf.Ceil(f);
    }

    int HardMaxValue(int turn) {
      const int TAPER_POINT = 10;
      return turn < TAPER_POINT ? Return(Piece1(turn)) : Return(Piece2(turn));

      float Piece1(float i) => Mathf.Pow(i, 0.9f) + 3;
      float Piece2(float i) {
        return Sub(i) + Piece1(TAPER_POINT) - Sub(TAPER_POINT);
        float Sub(float i) => 16 * Mathf.Log(i, 10);
      }
      int Return(float f) => (int)Mathf.Ceil(f);
    }
  }

  public void SetAttackData(AttackData data) {
    AttackData = data;
  }

  // methods

  private void HandleNewAnswer() => BattleCanvasMenuBattleInfo.Instance.SetAccuracy(CorrectAnswers, ++TotalAnswers);

  // base methods

  protected override void Awake() {
    base.Awake();
    UpdateDifficulty();
  }

  // get set

  public int Turn {
    get { return BattleRecord.turnCount; }
    private set { BattleRecord.turnCount = value; }
  }

  public int DefeatCount {
    get { return BattleRecord.defeatCount; }
    private set { BattleRecord.defeatCount = value; }
  }

  public int TotalAnswers {
    get { return BattleRecord.totalAnswers; }
    private set { BattleRecord.totalAnswers = value; }
  }

  public int CorrectAnswers {
    get { return BattleRecord.correctAnswers; }
    private set { BattleRecord.correctAnswers = value; }
  }

  public float AvgAnswerTime {
    get { return BattleRecord.avgAnswerTime; }
    private set { BattleRecord.avgAnswerTime = value; }
  }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMath : BattleStateBase
{

  private enum MyState
  {
    Idle, Answer, Result, Attack,
  }

  private Dictionary<MathOperation, string> MathOpSymbol = new Dictionary<MathOperation, string> {
    { MathOperation.Add, "+" },
    { MathOperation.Div, ":" },
    { MathOperation.Mul, "×" },
    { MathOperation.Sub, "-" },
  };

  private MathData param;

  private MyState myState;
  private string answer;
  private float answerTimer, answerGraceTimer, delayTimer;

  // state methods

  public override void EnterState<T>(T data) {
    param = BattleController.Instance.MathData;

    BattleCanvasFightSkillDescription.Instance.Hide();

    BattleUnitManager.Instance.HideAllUnitOverhead();
    BattleCameraRig.Instance.ToggleLockCamera(true);

    answer = "";

    answerTimer = param.maxTime;
    answerGraceTimer = param.graceTime;

    string questionText = string.Format("{0} {1} {2}",
      param.leftValue,
      MathOpSymbol[param.operation],
      param.rightValue);

    BattleCanvasMathPrompt.Instance.SetQuestion(questionText);
    BattleCanvasMathPrompt.Instance.SetAnswer("");
    BattleCanvasMathPrompt.Instance.InitTimer(param.critTime, param.maxTime);
    BattleCanvasMathPrompt.Instance.UpdateTimer(param.maxTime, param.maxTime);

    BattleCanvasMathPrompt.Instance.ToggleShow(true);

    myState = MyState.Answer;

  }

  public override void LeaveState(BattleState outgoingState) {
    BattleCameraRig.Instance.ToggleLockCamera(false);

    myState = MyState.Idle;
  }

  public override void UpdateState() {
    HandleAnswerState();
    HandleResultState();
  }

  // methods

  private void HandleAnswerState() {
    if (myState != MyState.Answer) return;

    HandleAnswerStateInput();
    HandleAnswerStateSubmit();

    if (answerGraceTimer > 0) {
      answerGraceTimer -= Time.deltaTime;
      return;
    }
    if (answerTimer > 0) {
      answerTimer -= Time.deltaTime;
      BattleCanvasMathPrompt.Instance.UpdateTimer(answerTimer, param.maxTime);
    } else {
      HandleAnswerStateSubmit(true);
    }
  }

  private void HandleAnswerStateInput() {
    UtilInput.NumberKeys.ForEach((key) => {
      if (Input.GetKeyDown(key) && answer.Length < 4) {
        int number = (int)key - (int)KeyCode.Alpha0;
        answer += number.ToString();
      }
    });
    if (Input.GetKeyDown(KeyCode.Backspace) && answer.Length > 0) {
      answer = answer.Remove(answer.Length - 1);
    }
    BattleCanvasMathPrompt.Instance.SetAnswer(answer);
  }

  private void HandleAnswerStateSubmit(bool timeUp = false) {
    if (answer == param.correctAnswer.ToString()) {
      BattleCanvasMathPrompt.Instance.SetAnswerCorrect(IsCrit);
      BattleController.Instance.HandleCorrectAnswer(
        param.graceTime - answerGraceTimer +
        param.maxTime - answerTimer);
      Continue();
    } else if (timeUp) {
      BattleCanvasMathPrompt.Instance.SetAnswerIncorrect();
      BattleController.Instance.HandleIncorrectAnswer();
      Continue();
    }

    void Continue() {
      myState = MyState.Result;
      delayTimer = 1;
    }
  }

  private void HandleResultState() {
    if (myState != MyState.Result) return;

    if (delayTimer > 0) {
      delayTimer -= Time.deltaTime;
    } else {
      HandleAttack();
    }
  }

  private void HandleAttack() {
    BattleUnitBase attacker = AttackData.attacker;

    float bonus;

    AttackData.isCrit = AttackData.isSuccessful && IsCrit;

    // bonus calc

    if (attacker.IsHero) {
      if (AttackData.isSuccessful) {
        bonus = IsCrit ? param.critBonus : 1;
      } else {
        bonus = 0;
      }
    } else {
      if (AttackData.isSuccessful) {
        bonus = IsCrit ? 1 : 0;
      } else {
        bonus = 1;
      }
    }

    // set attacker and targets


    if (!attacker.IsHero && AttackData.isSuccessful && IsCrit) {
      // if we crit defend against an enemy, turn the table
      List<BattleUnitBase> targetList = new List<BattleUnitBase>();
      targetList.Add(attacker);

      AttackData attackData = new AttackData {
        attacker = SelectedHero,
        targetList = targetList,
        attackerSkill = null,
      };
      BattleController.Instance.SetAttackData(attackData);

      SelectedHero.SetToAttack(targetList);
      SelectedHero.SetOverheadInfoAlpha(0);
    } else {
      List<BattleUnitBase> targetList = AttackData.targetList;

      attacker.SetToAttack(targetList);
      attacker.SetOverheadInfoAlpha(0);
    }

    BattleCanvasMathPrompt.Instance.ToggleShow(false);
    BattleUnitManager.Instance.BacklogDamageHit(param.damageMin, param.damageMax, bonus);

    myState = MyState.Attack;
  }

  // signals

  public override void Subscribe() { }

  public override void Unsubscribe() { }

  // init

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;
  }

  // util

  private bool IsCrit {
    get {
      return answerTimer >= param.maxTime - param.critTime;
    }
  }

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

  private AttackData AttackData {
    get {
      return BattleController.Instance.AttackData;
    }
  }

}

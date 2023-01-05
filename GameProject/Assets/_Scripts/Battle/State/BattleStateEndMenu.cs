using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EndMenuOptionId
{
  End_Turn,
  End_Battle,
}
public struct EndMenuOption
{
  public EndMenuOptionId idName;
  public string title;
  public string description;
}

public class BattleStateEndMenu : BattleStateBase
{

  private List<EndMenuOption> optionList = new List<EndMenuOption> {
    {
      new EndMenuOption {
        idName = EndMenuOptionId.End_Turn,
        title = "Akhiri giliran",
        description = "Akhiri giliran unitmu. Semua unit lawan akan mulai menyerang setelah ini.",
      }
    },
    {
      new EndMenuOption {
        idName = EndMenuOptionId.End_Battle,
        title = "Kabur",
        description = "Akhiri pertarungan ini dan catat skormu.",
      }
    },
  };
  private List<MenuListButtonData<EndMenuOption>> optionButtonList;

  // state methods

  public override void EnterState<T>(T data) {

    optionButtonList = GenerateButtonDataList();

    BattleCanvasMenuEndOptionList.Instance.Show(optionButtonList, false, "", OnScroll, OnSelect);
    BattleCanvasMenuEndOptionList.Instance.SetIndex(0); // make sure players don't accidentally end the battle

    BattleTileSelection.Instance.ToggleEnableSelectTile(false);
    BattleCanvasEndOptionDescription.Instance.Show();
  }

  public override void LeaveState(BattleState outgoingState) {
    BattleCanvasMenuEndOptionList.Instance.Hide();
    BattleCanvasEndOptionDescription.Instance.Hide();
  }

  public override void UpdateState() {
    HandleBackToMainMenu();
  }

  // methods

  private List<MenuListButtonData<EndMenuOption>> GenerateButtonDataList() {
    List<MenuListButtonData<EndMenuOption>> buttonDataList = new List<MenuListButtonData<EndMenuOption>>();
    
    foreach (EndMenuOption item in optionList) {
      MenuListButtonData<EndMenuOption> buttonData = new MenuListButtonData<EndMenuOption> {
        data = item,
        title = item.title,
      };
      buttonDataList.Add(buttonData);
    }

    return buttonDataList;
  }

  private void OnScroll(MenuListButtonData<EndMenuOption> hoveredOption) {
    BattleCanvasEndOptionDescription.Instance.SetInfo(hoveredOption.data.description);
  }

  private void OnSelect(MenuListButtonData<EndMenuOption> hoveredOption, int index) {
    switch (hoveredOption.data.idName) {
      case EndMenuOptionId.End_Turn:
        StateManager.SwitchState(BattleState.EnemyTurn);
        break;
      case EndMenuOptionId.End_Battle:
        StateManager.SwitchState(BattleState.GameOver);
        break;
      default:
        break;
    }
  }

  private void HandleBackToMainMenu() {
    if (
      CanvasSceneTransition.Instance.IsTransitioning ||
      !Input.GetKeyDown(GameKeyMapping.CancelKey)) return;

    StateManager.SwitchState(BattleState.MainMenu);
  }

  // signals

  public override void Subscribe() { }

  public override void Unsubscribe() { }

  // init

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;
  }

}

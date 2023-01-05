using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateItemMenu : BattleStateBase
{

  private const string EMPTY_TEXT = "Belum ada item yang dapat digunakan.";

  public static List<Item> ItemList { get; private set; }
  private List<MenuListButtonData<Item>> itemButtonList;

  public static DelegateVoid OnItemBackToMain;

  // state method

  public override void EnterState<T>(T data) {
    ItemList = BattleTileSelection.Instance.SelectedTile.Unit.ItemList;
    itemButtonList = GenerateButtonDataList();

    BattleCanvasMenuItemList.Instance.Show(itemButtonList, true, EMPTY_TEXT, OnScroll, OnSelect);
    BattleCanvasItemDescription.Instance.Show();

    BattleTileSelection.Instance.ToggleEnableSelectTile(false);
  }

  public override void LeaveState(BattleState outgoingState) {
    BattleCanvasMenuItemList.Instance.Hide();
    BattleCanvasItemDescription.Instance.Hide();
  }

  public override void UpdateState() {
    HandleBackToMainMenu();
  }

  // methods

  private List<MenuListButtonData<Item>> GenerateButtonDataList() {
    List<MenuListButtonData<Item>> buttonDataList = new List<MenuListButtonData<Item>>();

    foreach (Item item in ItemList) {
      MenuListButtonData<Item> buttonData = new MenuListButtonData<Item> {
        data = item,
        title = item.data.title,
        qty = item.qty,
      };
      buttonDataList.Add(buttonData);
    }

    return buttonDataList;
  }

  private void OnScroll(MenuListButtonData<Item> hoveredItem) {
    BattleCanvasItemDescription.Instance.SetInfo(hoveredItem.data.data.description);
  }

  private void OnSelect(MenuListButtonData<Item> hoveredItem, int index) {
    HandleUseItem(hoveredItem.data, index);
  }

  private void HandleBackToMainMenu() {
    if (!Input.GetKeyDown(GameKeyMapping.CancelKey)) return;

    StateManager.SwitchState(BattleState.MainMenu);
  }

  // signals

  public override void Subscribe() { }

  public override void Unsubscribe() { }

  // init

  public override void Init(BattleStateManager stateManager) {
    StateManager = stateManager;
  }

  // util

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

  // handle item usage

  private void HandleUseItem(Item selectedItem, int index) {
    ItemScriptable data = selectedItem.data;
    string itemUseText = "";

    int a;
    switch (data.idName) {

      case "ap plus":
        a = 10;

        SelectedHero.AP += a;
        itemUseText = string.Format("Kamu mendapat {0} <color=#d3ff00>AP</color>!", a);
        break;
      case "hp plus":
        a = 10;

        SelectedHero.HP += a;
        itemUseText = string.Format("Kamu mendapat {0} <color=#f59a95>HP!</color>", a);
        break;
      default:
        break;
    }

    if (selectedItem.qty > 0) {
      Item temp = ItemList[index];
      temp.qty--;
      ItemList[index] = temp;

      selectedItem = temp;
    }
    if (selectedItem.qty == 0) {
      ItemList.RemoveAt(index);
    }

    BattleCanvasMenuItemList.Instance.Refresh(GenerateButtonDataList());
    BattleCanvasInfoBanner.Instance.Show(itemUseText, 3);
  }

}

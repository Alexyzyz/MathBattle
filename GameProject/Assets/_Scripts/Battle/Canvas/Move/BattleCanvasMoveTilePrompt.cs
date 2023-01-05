using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasMoveTilePrompt : Singleton<BattleCanvasMoveTilePrompt>
{

  private CanvasGroup promptContainer;
  private CanvasGroup movePrompt;

  private Text movePromptText; 

  // public methods

  public void Show() => ToggleShow(true);
  public void Hide() => ToggleShow(false);

  public void ToggleSelectPromptAlpha(BattleTile selectedTile) {
    movePrompt.alpha =
      selectedTile &&
      SelectedHero &&
      SelectedHero.AP >= UtilData.BATTLE_BASE_MOVE_COST ?
      1 : 0.5f;
  }

  // methods

  private void ToggleShow(bool show) {
    promptContainer.alpha = show ? 1 : 0;
  }

  // init

  private void Init() {
    promptContainer = GetComponent<CanvasGroup>();
    movePrompt = transform.Find("MovePrompt").GetComponent<CanvasGroup>();
    movePromptText = transform.Find("MovePrompt/Text").GetComponent<Text>();

    promptContainer.alpha = 0;
    movePrompt.alpha = 0.3f;
    movePromptText.text = string.Format("Pindah <color=#D3FF00>-{0} AP</color>", UtilData.BATTLE_BASE_MOVE_COST);
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  // util

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasFightTargetTilePrompt : Singleton<BattleCanvasFightTargetTilePrompt>
{

  private const string PROMPT_TEXT = "Serang <color=#D3FF00>-{0} AP</color>";

  private CanvasGroup promptContainer;
  private CanvasGroup targetPrompt;
  private Text targetPromptText;

  // public methods

  public void Show() => ToggleShow(true);

  public void Hide() => ToggleShow(false);

  public void SetSkillCost(int cost) => targetPromptText.text = string.Format(PROMPT_TEXT, cost);

  public void ToggleSelectPromptAlpha(bool isValid) {
    targetPrompt.alpha = isValid ? 1 : 0.5f;
  }

  // methods

  private void ToggleShow(bool show) {
    promptContainer.alpha = show ? 1 : 0;
  }

  // util

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

  // init

  private void Init() {
    promptContainer = GetComponent<CanvasGroup>();
    targetPrompt = transform.Find("TargetPrompt").GetComponent<CanvasGroup>();
    targetPromptText = transform.Find("TargetPrompt/Text").GetComponent<Text>();

    promptContainer.alpha = 0;
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

}

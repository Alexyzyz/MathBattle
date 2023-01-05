using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TitleMainListButtonData
{
  public int index;
  public TitleState idName;
  public string text;
}

public class TitleCanvasMainList : Singleton<TitleCanvasMainList>
{

  private List<TitleMainListButtonData> buttonDataList = new List<TitleMainListButtonData> {
    new TitleMainListButtonData {
      index = 0,
      idName = TitleState.Start,
      text = "Mulai",
    },
    new TitleMainListButtonData {
      index = 1,
      idName = TitleState.Tutorial,
      text = "Tutorial",
    },
    new TitleMainListButtonData {
      index = 2,
      idName = TitleState.History,
      text = "Sejarah",
    },
    new TitleMainListButtonData {
      index = 3,
      idName = TitleState.Quit,
      text = "Keluar",
    }
  };

  [SerializeField]
  private TitleCanvasMainListButton buttonPrefab;

  public bool IsActive { get; set; } = true;

  private int currIndex = 0;

  public static DelegateInt OnHoverOption;

  // methods

  private void HandleSelect() {
    if (
      CanvasSceneTransition.Instance.IsTransitioning ||
      !Input.GetKeyDown(GameKeyMapping.ConfirmKey)) return;

    if (HoveredOption == TitleState.Start) {
      // start game
      CanvasSceneTransition.Instance.TransitionToScene(CoverTransition.Right, CoverTransition.Center, "BattleScene");
    } else
    if (HoveredOption == TitleState.Tutorial) {
      // tutorial
      TitleStateManager.Instance.SwitchState(TitleState.Tutorial);
    } else
    if (HoveredOption == TitleState.History) {
      // history
      TitleStateManager.Instance.SwitchState(TitleState.History);
    }
    if (HoveredOption == TitleState.Quit) {
      // quit
      Application.Quit();
    }
  }

  private void HandleScrollInput() {
    if (CanvasSceneTransition.Instance.IsTransitioning) return;

    int direction =
      (Input.GetKeyDown(GameKeyMapping.UpKey) ? -1 : 0) +
      (Input.GetKeyDown(GameKeyMapping.DownKey) ? 1 : 0);

    if (direction == 0) return;

    currIndex = Mathf.Clamp(currIndex + direction, 0, buttonDataList.Count - 1);
    OnHoverOption?.Invoke(currIndex);
  }

  // init

  private void Init() {
    Vector2 newButtonPos = Vector2.zero;
    foreach (TitleMainListButtonData buttonData in buttonDataList) {
      TitleCanvasMainListButton newButton = Instantiate(buttonPrefab, newButtonPos, Quaternion.identity, transform);
      newButton.GetComponent<RectTransform>().anchoredPosition = newButtonPos;
      newButton.SetData(buttonData);

      newButtonPos -= new Vector2(0, 70);
    }
  }

  // util

  private TitleState HoveredOption {
    get {
      return buttonDataList[currIndex].idName;
    }
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  private void Start() {
    // invoke button immediately
    OnHoverOption?.Invoke(0);
  }

  private void Update() {
    if (!IsActive) return;
    HandleScrollInput();
    HandleSelect();
  }


}

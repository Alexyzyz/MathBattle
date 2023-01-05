using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct MenuListButtonData<T>
{
  public T data;
  public string title;
  public int qty;
  public bool isGrayed;
}

public class BattleCanvasMenuListBase<T> : MonoBehaviour, SubscriberInterface
{

  [SerializeField] private BattleCanvasMenuListButtonBase buttonPrefab;

  private RectTransform myRect;
  private RectTransform myListRect;
  private CanvasResizableRect myScrollbar;
  private RectTransform myScrollbarRect;
  private RectTransform myScrollbarTrackRect;

  private CanvasGroup myUnemptyPromptGroup;
  private RectTransform myUnemptyPromptRect;

  private CanvasGroup myEmptyPromptGroup;
  private Text myEmptyPromptText;

  private List<BattleCanvasMenuListButtonBase> buttonList = new List<BattleCanvasMenuListButtonBase>();
  private List<MenuListButtonData<T>> buttonDataList = new List<MenuListButtonData<T>>();

  private int currIndex {
    get {
      return Mathf.Clamp(_currIndex, 0, buttonDataList.Count - 1);
    }
    set {
      _currIndex = Mathf.Clamp(value, 0, buttonDataList.Count - 1);
    }
  }
  private int _currIndex;

  private Vector2 containerTargetPos;
  private Vector2 listTargetPos;

  private int maxItemsShown = 5;
  private float itemHeight = 40;
  private float itemMargin = 10;
  private float promptMargin = 20;
  private float scrollBarHeight;

  private bool isShown;
  private bool showQty;

  private string emptyText;

  // for some reason confirm input can leak
  private bool firstConfirmPressed;

  private Action<MenuListButtonData<T>> OnScroll;
  private Action<MenuListButtonData<T>, int> OnSelect;

  public static DelegateInt OnHoverOption;

  // public methods

  public void Show(
    List<MenuListButtonData<T>> buttonDataList,
    bool showQty,
    string emptyText,
    Action<MenuListButtonData<T>> onScroll,
    Action<MenuListButtonData<T>, int> onSelect)
  {

    this.buttonDataList = buttonDataList;

    this.showQty = showQty;
    this.emptyText = emptyText;

    OnScroll = onScroll;
    OnSelect = onSelect;

    containerTargetPos.x = AnimState.Shown.posX;

    GenerateList();
    HandleScroll();

    isShown = true;

  }

  public void Hide() {
    containerTargetPos.x = AnimState.Hidden.posX;

    isShown = false;
    firstConfirmPressed = false;

    buttonList.ForEach((item) => Destroy(item.gameObject));
    buttonList.Clear();
  }

  public void Refresh(List<MenuListButtonData<T>> buttonDataList) {
    this.buttonDataList = buttonDataList;

    buttonList.ForEach((item) => Destroy(item.gameObject));
    buttonList.Clear();

    GenerateList();
    HandleScroll();
  }

  public void SetIndex(int index) {
    currIndex = index;
    OnHoverOption?.Invoke(currIndex);
  }

  // methods

  private void GenerateList() {

    bool showScrollbar = buttonDataList.Count > maxItemsShown;

    // 1  Set whether scrollbar should be visible

    myScrollbarRect.gameObject.SetActive(showScrollbar);
    myScrollbarTrackRect.gameObject.SetActive(showScrollbar);

    // 2  Set scrollbar and track height (if visible)

    float listHeight = Mathf.Min(buttonDataList.Count, maxItemsShown) * (itemHeight + itemMargin) - itemMargin;
    float scrollbarHeight = (maxItemsShown * (itemHeight + itemMargin) - itemMargin) / Mathf.Max(buttonDataList.Count, 1);

    if (showScrollbar) {      
      scrollBarHeight = myScrollbar.SetLength(scrollbarHeight);
      myScrollbarTrackRect.sizeDelta = new Vector2(myScrollbarTrackRect.rect.width, listHeight);
    }

    // 3  Set empty text and prompts accordingly
    
    if (buttonDataList.Count > 0) {
      myEmptyPromptText.text = "";
      
      myUnemptyPromptRect.anchoredPosition = new Vector2(0, -listHeight - promptMargin);

      myEmptyPromptGroup.alpha = 0;
      myUnemptyPromptGroup.alpha = 1;
    } else {
      myEmptyPromptText.text = emptyText;

      myEmptyPromptGroup.alpha = 1;
      myUnemptyPromptGroup.alpha = 0;

      return;
    }

    // 4  Instantiate buttons

    for (int i = 0; i < buttonDataList.Count; i++) {
      MenuListButtonData<T> item = buttonDataList[i];
      Vector3 startPos = new Vector3(0, -(itemHeight + itemMargin) * i);

      BattleCanvasMenuListButtonBase button = Instantiate(
        buttonPrefab,
        Vector3.zero,
        Quaternion.identity,
        myListRect.transform);
      button.GetComponent<RectTransform>().anchoredPosition = startPos;

      button.SetData(i, item.title, showQty, item.qty, IsGrayedOut(item.data), buttonDataList.Count, maxItemsShown);
      buttonList.Add(button);
    }
  }

  private void HandleSelectInput() {
    if (CanvasSceneTransition.Instance.IsTransitioning) return;
    if (!firstConfirmPressed) {
      firstConfirmPressed = true;
      return;
    }
    if (buttonDataList.Count == 0) return;
    if (Input.GetKeyDown(GameKeyMapping.ConfirmKey)) {
      OnSelect(HoveredButton, currIndex);
    }
  }

  protected void HandleScroll() {
    if (buttonDataList.Count == 0) return;

    int viewScrollIndex = Mathf.Clamp(
      currIndex,
      maxItemsShown / 2,
      buttonList.Count - 1 - maxItemsShown / 2
    );

    float scrollTargetY = (viewScrollIndex - maxItemsShown / 2) * (itemHeight + itemMargin);
    listTargetPos.y = scrollTargetY;

    Vector3 newScrollbarPos = myScrollbarRect.anchoredPosition;
    float scrollbarTravelPercent = currIndex / (float)(buttonList.Count - 1);

    newScrollbarPos.y = -scrollbarTravelPercent * (maxItemsShown * (itemHeight + itemMargin) - scrollBarHeight - itemMargin);

    myScrollbarRect.anchoredPosition = newScrollbarPos;

    OnScroll(HoveredButton);
    OnHoverOption?.Invoke(currIndex);
  }

  private void HandleScrollInput() {
    if (CanvasSceneTransition.Instance.IsTransitioning) return;

    int direction =
      (Input.GetKeyDown(GameKeyMapping.UpKey) ? -1 : 0) +
      (Input.GetKeyDown(GameKeyMapping.DownKey) ? 1 : 0);

    if (direction == 0) return;

    currIndex = Mathf.Clamp(currIndex + direction, 0, buttonDataList.Count - 1);

    HandleScroll();
  }

  protected virtual bool IsGrayedOut(T data) => false;

  // signals

  public virtual void Subscribe() { }

  public virtual void Unsubscribe() { }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
    myListRect = transform.Find("ListContainer/List").GetComponent<RectTransform>();
    myScrollbarTrackRect = transform.Find("ScrollbarContainer/ScrollbarTrack").GetComponent<RectTransform>();

    myUnemptyPromptRect = transform.Find("PromptContainer/UnemptyList").GetComponent<RectTransform>();
    myUnemptyPromptGroup = transform.Find("PromptContainer/UnemptyList").GetComponent<CanvasGroup>();
    
    myEmptyPromptGroup = transform.Find("PromptContainer/EmptyList").GetComponent<CanvasGroup>();
    myEmptyPromptText = transform.Find("PromptContainer/EmptyList/Text").GetComponent<Text>();

    Vector2 startPos = new Vector2(AnimState.Hidden.posX, myRect.anchoredPosition.y);
    myRect.anchoredPosition = startPos;
    containerTargetPos = startPos;

    Transform myScrollbarTransform = transform.Find("ScrollbarContainer/Scrollbar");
    myScrollbar = myScrollbarTransform.GetComponent<CanvasResizableRect>();
    myScrollbarRect = myScrollbarTransform.GetComponent<RectTransform>();

    StartCoroutine(AnimateContainerToTargetPos());
    StartCoroutine(AnimateListToTargetPos());
  }

  // util

  private MenuListButtonData<T> HoveredButton {
    get {
      return buttonDataList[currIndex];
    }
  }

  // base methods

  protected virtual void Awake() {
    Init();
    Subscribe();
  }

  private void Update() {
    if (!isShown) return;
    HandleScrollInput();
    HandleSelectInput();
  }

  private void OnDestroy() {
    Unsubscribe();
  }



  // animations

  private struct AnimStateData
  {
    public float posX;
  }

  private class AnimStateBase
  {
    public AnimStateData Shown = new AnimStateData {
      posX = 0,
    };
    public AnimStateData Hidden = new AnimStateData {
      posX = -500,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator AnimateContainerToTargetPos() {
    while (true) {
      if (Vector2.Distance(myRect.anchoredPosition, containerTargetPos) > 0.01f) {
        myRect.anchoredPosition = Vector2.Lerp(
          myRect.anchoredPosition,
          containerTargetPos,
          0.2f
        );
      }
      else {
        myRect.anchoredPosition = containerTargetPos;
      }
      yield return null;
    }
  }

  private IEnumerator AnimateListToTargetPos() {
    while (true) {
      if (Vector2.Distance(myListRect.anchoredPosition, listTargetPos) > 0.01f) {
        myListRect.anchoredPosition = Vector2.Lerp(
          myListRect.anchoredPosition,
          listTargetPos,
          0.2f
        );
      }
      else {
        myListRect.anchoredPosition = listTargetPos;
      }
      yield return null;
    }
  }

}

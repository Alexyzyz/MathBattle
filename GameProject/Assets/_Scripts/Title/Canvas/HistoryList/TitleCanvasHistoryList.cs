using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleCanvasHistoryList : Singleton<TitleCanvasHistoryList>
{

  private const float BUTTON_HEIGHT = 60;
  private const float BUTTON_MARGIN = 3;
  private const int SHOWN_BUTTON_COUNT = 5;

  [SerializeField]
  private GameObject buttonPrefab;

  private RectTransform myContainerRect;
  private CanvasGroup myTextCanvasGroup;

  private List<GameObject> buttonList = new List<GameObject>();

  private bool isActive;
  private float maxYpos;

  // public methods

  public void Deactivate() => isActive = false;

  public void Activate(List<BattleRecord> buttonDataList) {
    buttonList.ForEach((item) => Destroy(item));

    myTextCanvasGroup.alpha = buttonDataList.Count == 0 ? 1 : 0;

    Vector2 buttonPos = Vector2.zero;
    buttonDataList.ForEach((item) => {
      GameObject newButton = Instantiate(buttonPrefab, buttonPos, Quaternion.identity, myContainerRect.transform);
      newButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;

      float sanitizedAccuracy = item.totalAnswers == 0 ? 0 : 100 * (float)item.correctAnswers / item.totalAnswers;

      string datetime = string.Format("{0}  {1}", item.datetime.ToShortDateString(), item.datetime.ToShortTimeString());
      string accuracy = string.Format("{0:0.0}%", sanitizedAccuracy);
      string answer = string.Format("{0} / {1} / {2}", item.correctAnswers, item.totalAnswers - item.correctAnswers, item.totalAnswers);
      string time = string.Format("{0:0.000}s", item.avgAnswerTime);

      newButton.transform.Find("Date").GetComponent<Text>().text = datetime;
      newButton.transform.Find("Turn").GetComponent<Text>().text = item.turnCount.ToString();
      newButton.transform.Find("Defeat").GetComponent<Text>().text = item.defeatCount.ToString();
      newButton.transform.Find("Accuracy").GetComponent<Text>().text = item.totalAnswers > 0 ? accuracy : "—";
      newButton.transform.Find("Answer").GetComponent<Text>().text = answer;
      newButton.transform.Find("Time").GetComponent<Text>().text = item.totalAnswers > 0 ? time : "—";

      buttonList.Add(newButton);

      buttonPos -= new Vector2(0, BUTTON_HEIGHT + BUTTON_MARGIN);
    });
    maxYpos = buttonPos.y + BUTTON_HEIGHT + BUTTON_MARGIN;

    isActive = true;
  }

  // methods

  private void HandleScroll() {
    int direction =
      (Input.GetKey(GameKeyMapping.UpKey) ? -1 : 0) +
      (Input.GetKey(GameKeyMapping.DownKey) ? 1 : 0);

    if (direction == 0) return;

    float nextY = myContainerRect.anchoredPosition.y + 10 * direction;
    nextY = Mathf.Clamp(nextY, 0, -maxYpos);

    Vector2 nextPos = new Vector2(0, nextY);
    myContainerRect.anchoredPosition = nextPos;
  }

  // init

  private void Init() {
    myContainerRect = transform.Find("Container").GetComponent<RectTransform>();
    myTextCanvasGroup = transform.Find("EmptyText").GetComponent<CanvasGroup>();
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  private void Update() {
    if (!isActive) return;
    HandleScroll();
  }

}

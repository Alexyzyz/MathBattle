using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleCanvasTutorialList : Singleton<TitleCanvasTutorialList>
{

  private const float SLIDE_WIDTH = 800;

  [SerializeField]
  private List<TutorialSlideScriptable> buttonDataList;

  [SerializeField]
  private GameObject slidePrefab;

  private Text myTitle;
  private RectTransform myCarouselRect;

  private bool isActive;
  private int currIndex = 0;

  // public methods

  public void Activate() => isActive = true;
  public void Deactivate() => isActive = false;

  // methods

  private void GenerateCarousel() {
    Vector2 buttonPos = Vector2.zero;
    buttonDataList.ForEach((item) => {
      GameObject newSlide = Instantiate(slidePrefab, buttonPos, Quaternion.identity, myCarouselRect.transform);
      newSlide.GetComponent<RectTransform>().anchoredPosition = buttonPos;

      newSlide.transform.Find("Mask/Image").GetComponent<Image>().sprite = item.image;
      newSlide.transform.Find("Text").GetComponent<Text>().text = item.description;

      buttonPos += new Vector2(SLIDE_WIDTH, 0);
    });
    TitleCanvasTutorialButtonList.Instance.GenerateList(buttonDataList.Count);

    myTitle.text = buttonDataList[currIndex].title;
  }

  private void HandleScroll() {
    myTitle.text = buttonDataList[currIndex].title;

    Vector2 to = new Vector2(currIndex * -SLIDE_WIDTH, 0);

    UtilCoroutine.Instance.StartCoroutine(this, ref animSlide, AnimateSlide(to));

    TitleCanvasTutorialButtonList.Instance.SetHoveredButton(currIndex);
  }

  private void HandleScrollInput() {
    int direction =
      (Input.GetKeyDown(GameKeyMapping.RightKey) ? 1 : 0) +
      (Input.GetKeyDown(GameKeyMapping.LeftKey) ? -1 : 0);

    if (direction == 0) return;

    currIndex = Mathf.Clamp(currIndex + direction, 0, buttonDataList.Count - 1);
    HandleScroll();
  }

  // init

  private void Init() {
    myTitle = transform.Find("Title").GetComponent<Text>();
    myCarouselRect = transform.Find("CarouselMask/Carousel").GetComponent<RectTransform>();

    GenerateCarousel();
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  private void Update() {
    if (!isActive) return;
    HandleScrollInput();
  }



  // animation

  private IEnumerator animSlide;

  private IEnumerator AnimateSlide(Vector2 to) {
    Vector2 from = myCarouselRect.anchoredPosition;

    float val = 0;
    while (val < 0.999f) {
      UpdateProps();
      val = Mathf.Lerp(val, 1, 0.2f);
      yield return null;
    }
    val = 1;
    UpdateProps();

    void UpdateProps() {
      myCarouselRect.anchoredPosition = Vector2.Lerp(from, to, val);
    }
  }

}

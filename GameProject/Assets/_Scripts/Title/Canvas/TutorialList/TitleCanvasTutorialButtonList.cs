using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCanvasTutorialButtonList : Singleton<TitleCanvasTutorialButtonList>
{

  private const float BUTTON_WIDTH = 10;
  private const float BUTTON_MARGIN = 10;

  [SerializeField]
  private GameObject buttonPrefab;

  private List<GameObject> buttonList = new List<GameObject>();

  private int currIndex;

  // public methods

  public void SetHoveredButton(int index) {
    SetButtonHoveredState(currIndex, false);
    SetButtonHoveredState(index, true);
    currIndex = index;
  }

  public void GenerateList(int count) {
    Vector2 buttonPos = new Vector2(-(BUTTON_WIDTH + BUTTON_MARGIN) * count / 2f - BUTTON_MARGIN, 0);
    int i = count;
    while (i > 0) {
      GameObject newButton = Instantiate(buttonPrefab, buttonPos, Quaternion.identity, transform);
      newButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;

      buttonList.Add(newButton);

      buttonPos += new Vector2(BUTTON_WIDTH + BUTTON_MARGIN, 0);
      i--;
    }
    SetHoveredButton(0);
  }

  // methods

  private void SetButtonHoveredState(int index, bool hovered) {
    buttonList[index].transform.Find("Dot").GetComponent<CanvasGroup>().alpha = hovered ? 1 : 0.3f;
  }

}

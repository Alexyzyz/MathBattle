using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasMenuMainButton : MonoBehaviour, SubscriberInterface
{

  private Transform menuMainContainer;
  private Transform menuActionHeaderContainer;

  private RectTransform myRect;
  private RectTransform myBackground;
  private RectTransform myText;
  private Image myBackgroundImage;

  private bool isSelected;
  private bool isHovered;

  [SerializeField] private BattleState myMainMenuOption;
  [SerializeField] private Color hoveredColor;

  // signal methods

  private void HandleOnBackToMainMenu() {
    ResetParent();
    UtilCoroutine.Instance.StartCoroutine(this, ref currAnim, AnimateSelected());
  }

  private void ResetParent() {
    isSelected = false;
    transform.SetParent(menuMainContainer);
  }

  private void HandleOnSelected(BattleState selectedOption) {
    isSelected = myMainMenuOption == selectedOption;
    if (isSelected) {
      transform.SetParent(menuActionHeaderContainer);
      myRect.SetAsLastSibling();

      UtilCoroutine.Instance.StartCoroutine(this, ref currAnim, AnimateSelected());
    }
  }

  private void HandleOnHovered(BattleState hoveredOption) {
    isHovered = myMainMenuOption == hoveredOption;

    IEnumerator animToPlay = AnimateIdle();
    if (!isSelected && isHovered) {
      myRect.SetAsLastSibling();
      animToPlay = AnimateHovered();
    }
    UtilCoroutine.Instance.StartCoroutine(this, ref currAnim, animToPlay);
  }

  // signals

  public void Subscribe() {
    BattleStateMainMenu.OnBackToMainMenu += HandleOnBackToMainMenu;
    BattleStateMainMenu.OnSelectMainMenuOption += HandleOnSelected;
    BattleStateMainMenu.OnHoverMainMenuOption += HandleOnHovered;

    BattleStateEnemyTurn.OnEnemyTurn += ResetParent;
    BattleStateGameOver.OnGameOver += ResetParent;
  }

  public void Unsubscribe() {
    BattleStateMainMenu.OnBackToMainMenu -= HandleOnBackToMainMenu;
    BattleStateMainMenu.OnSelectMainMenuOption -= HandleOnSelected;
    BattleStateMainMenu.OnHoverMainMenuOption -= HandleOnHovered;

    BattleStateEnemyTurn.OnEnemyTurn -= ResetParent;
    BattleStateGameOver.OnGameOver -= ResetParent;
  }

  // init

  private void Init() {
    menuMainContainer = transform.parent;
    menuActionHeaderContainer = transform.parent.parent.Find("MenuHeaderIconContainer");

    myRect = GetComponent<RectTransform>();

    myBackground = transform.Find("Back").GetComponent<RectTransform>();
    myText = transform.Find("Text").GetComponent<RectTransform>();

    myBackgroundImage = myBackground.GetComponent<Image>();

    AnimState.Idle.pos = myRect.anchoredPosition;
    AnimState.Hovered.color = hoveredColor;
    AnimState.Selected.color = hoveredColor;
  }

  private void  InitCoroutines() {
    UtilCoroutine.Instance.StartCoroutine(this, ref currAnim, AnimateIdle());

    StartCoroutine(AnimateTextSway());
  }

  // base methods

  private void Awake() {
    Init();
  }

  private void Start() {
    InitCoroutines();
  }

  private void OnEnable() {
    Subscribe();
  }

  private void OnDestroy() {
    Unsubscribe();
  }

  // animations

  private struct AnimStateData
  {
    public Vector3 pos;
    public Vector3 rotation;
    public Vector3 scale;
    public Color color;
  }

  private class AnimStateBase {
    public AnimStateData Idle = new AnimStateData {
      rotation = Vector3.zero,
      scale = Vector3.one,
      color = new Color(0, 0, 0, 1),
    };
    public AnimStateData Hovered = new AnimStateData {
      pos = Vector3.zero,
      rotation = new Vector3(0, 0, 45),
      scale = 1.5f * Vector3.one,
    };
    public AnimStateData Selected = new AnimStateData {
      pos = Vector3.zero,
      rotation = new Vector3(0, 0, 22.5f),
      scale = 1.25f * Vector3.one,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private const float TEXT_SWAY_MAGNITUDE = 5f;
  private const float TEXT_SWAY_SPEED = 4f;
  private const float DAMP = 0.3f;

  private IEnumerator currAnim;

  private IEnumerator AnimateIdle() {
    while (true) {
      myRect.anchoredPosition = Vector3.Lerp(myRect.anchoredPosition, AnimState.Idle.pos, DAMP);
      myRect.localScale = Vector3.Lerp(myRect.localScale, AnimState.Idle.scale, DAMP);

      myBackground.eulerAngles = Vector3.Lerp(myBackground.eulerAngles, AnimState.Idle.rotation, DAMP);
      myBackgroundImage.color = Color.Lerp(myBackgroundImage.color, AnimState.Idle.color, DAMP);
      yield return null;
    }
  }

  private IEnumerator AnimateHovered() {
    while (true) {
      myRect.anchoredPosition = Vector3.Lerp(myRect.anchoredPosition, AnimState.Hovered.pos, DAMP);
      myRect.localScale = Vector3.Lerp(myRect.localScale, AnimState.Hovered.scale, DAMP);

      myBackground.eulerAngles = Vector3.Lerp(myBackground.eulerAngles, AnimState.Hovered.rotation, DAMP);
      myBackgroundImage.color = Color.Lerp(myBackgroundImage.color, AnimState.Hovered.color, DAMP);
      yield return null;
    }
  }

  private IEnumerator AnimateSelected() {
    while (true) {
      myRect.anchoredPosition = Vector3.Lerp(myRect.anchoredPosition, AnimState.Selected.pos, DAMP);
      myRect.localScale = Vector3.Lerp(myRect.localScale, AnimState.Selected.scale, DAMP);

      myBackground.eulerAngles = Vector3.Lerp(myBackground.eulerAngles, AnimState.Selected.rotation, DAMP);
      myBackgroundImage.color = Color.Lerp(myBackgroundImage.color, AnimState.Selected.color, DAMP);
      yield return null;
    }
  }

  private IEnumerator AnimateTextSway() {
    float textSwayTimer = 0f;
    while (true) {
      if (isHovered) {
        myText.eulerAngles = new Vector3(0, 0, TEXT_SWAY_MAGNITUDE * Mathf.Sin(textSwayTimer));
        textSwayTimer = (textSwayTimer + TEXT_SWAY_SPEED * Time.deltaTime) % 360f;
      }
      else {
        myText.eulerAngles = Vector3.zero;
      }
      yield return null;
    }
  }

}

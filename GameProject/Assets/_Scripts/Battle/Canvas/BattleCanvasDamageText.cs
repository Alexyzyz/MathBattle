using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasDamageText : MonoBehaviour
{

  private const string DEFAULT_TEXT = "-{0} HP";
  private const string HEAL_TEXT = "+{0} HP";
  private const string MISS_TEXT = "Terblokir!";

  private RectTransform myRect;
  private CanvasGroup myCanvasGroup;
  private Text myText;

  private float yOffset;

  // public methods

  public void SetDamageText(int damage) {
    string damageText;
    if (damage > 0) {
      damageText = DEFAULT_TEXT;
    } else if (damage == 0) {
      damageText = MISS_TEXT;
    } else {
      damageText = HEAL_TEXT;
    }
    myText.text = string.Format(damageText, damage.ToString());

    myRect.anchoredPosition = Vector2.zero;
  }

  // init

  private void Init() {
    myRect = GetComponent<RectTransform>();
    myCanvasGroup = GetComponent<CanvasGroup>();
    myText = GetComponent<Text>();

    myRect.localScale = Vector2.one * AnimState.Start.scale;

    UtilCoroutine.Instance.StartCoroutine(this, ref animState, AnimateIn());
  }

  // base methods

  private void Awake() {
    Init();
  }



  // animations

  private struct AnimStateData
  {
    public float scale;
    public float alpha;
    public float yOffset;
  }

  private class AnimStateBase {
    public AnimStateData Start = new AnimStateData {
      scale = 2f,
      alpha = 0,
      yOffset = 0,
    };
    public AnimStateData Shown = new AnimStateData {
      scale = 1,
      alpha = 1,
      yOffset = 0,
    };
    public AnimStateData Hidden = new AnimStateData {
      scale = 1,
      alpha = 0,
      yOffset = -80,
    };
  }
  private AnimStateBase AnimState = new AnimStateBase();

  private IEnumerator animState;

  // animators

  private IEnumerator AnimateIn() {
    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.2f);
      yield return null;
    }
    value = 1;
    UpdateProps();


    yield return new WaitForSeconds(2);
    UtilCoroutine.Instance.StartCoroutine(this, ref animState, AnimateOut());

    void UpdateProps() {
      myRect.localScale = Vector2.one * Mathf.Lerp(AnimState.Start.scale, AnimState.Shown.scale, value);
      myCanvasGroup.alpha = Mathf.Lerp(AnimState.Start.alpha, AnimState.Shown.alpha, value);
    }
  }

  private IEnumerator AnimateOut() {
    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.2f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    Destroy(gameObject);

    void UpdateProps() {
      yOffset = Mathf.Lerp(AnimState.Shown.yOffset, AnimState.Hidden.yOffset, value);
      myCanvasGroup.alpha = Mathf.Lerp(AnimState.Shown.alpha, AnimState.Hidden.alpha, value);

      myRect.anchoredPosition = new Vector2(0, -yOffset);
    }
  }

}

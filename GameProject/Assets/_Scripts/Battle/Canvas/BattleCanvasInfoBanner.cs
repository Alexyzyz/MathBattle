using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasInfoBanner : Singleton<BattleCanvasInfoBanner>
{

  private Animator myAnimator;
  private Text myText;

  private float time;

  // public methods

  public void Show(string text) {
    myText.text = text;

    myAnimator.Play("Enter", 0, 0);
  }

  public void Show(string text, float time) {
    myText.text = text;
    this.time = time;

    myAnimator.Play("Enter", 0, 0);

    UtilCoroutine.Instance.StartCoroutine(this, ref timerLeave, TimerLeave());
  }

  public void Hide() {
    myAnimator.Play("Leave", 0, 0);
  }

  // init

  private void Init() {
    myAnimator = GetComponent<Animator>();
    myText = transform.Find("Text").GetComponent<Text>();

    myAnimator.Play("Leave", 0, 1);
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }



  // timers

  private IEnumerator timerLeave;

  private IEnumerator TimerLeave() {
    yield return new WaitForSeconds(time);
    Hide();
  }

}

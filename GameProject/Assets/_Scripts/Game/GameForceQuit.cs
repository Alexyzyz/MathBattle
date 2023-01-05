using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameForceQuit : MonoBehaviour
{

  private const string HOLD_TEXT = "Tahan ESC selama {0:0.0}s lagi untuk keluar dari game.";
  private const float QUIT_TIME = 4f;
  private const float BACK_FADE_TIME = 2f;
  private const float TEXT_APPEAR_TIME = 0.2f;

  private CanvasGroup myBackCanvasGroup;
  private CanvasGroup myTextCanvasGroup;
  private Text myText;

  private float timer = QUIT_TIME;

  // methods

  private void HandleHold() {

    myBackCanvasGroup.alpha = Mathf.Lerp(0, 1, 1 - (Mathf.Max(QUIT_TIME - BACK_FADE_TIME, timer) - (QUIT_TIME - BACK_FADE_TIME)) / BACK_FADE_TIME);
    myTextCanvasGroup.alpha = Mathf.Lerp(0, 1, (QUIT_TIME - Mathf.Max(QUIT_TIME - TEXT_APPEAR_TIME, timer)) / TEXT_APPEAR_TIME );
    myText.text = string.Format(HOLD_TEXT, timer);

    // 4 => 1
    // 3 => 0 / 3

    if (Input.GetKey(KeyCode.Escape)) {
      if (timer > 0) {
        timer -= Time.deltaTime;
      } else {
        Application.Quit();
      }
      return;
    }

    timer = Mathf.Min(timer + 2.5f * Time.deltaTime, QUIT_TIME);
  }

  // init

  private void Init() {
    myBackCanvasGroup = transform.Find("Back").GetComponent<CanvasGroup>();
    myTextCanvasGroup = transform.Find("Text").GetComponent<CanvasGroup>();
    myText = transform.Find("Text").GetComponent<Text>();
  }

  // base methods

  private void Awake() {
    Init();
  }

  private void Update() {
    HandleHold();
  }

}

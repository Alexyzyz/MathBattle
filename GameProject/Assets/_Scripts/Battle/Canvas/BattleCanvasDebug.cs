using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCanvasDebug : Singleton<BattleCanvasDebug>
{

  private Text myText;

  public float updateInterval = 0.5f;

  float accum = 0.0f;
  int frames = 0;
  float timeleft;
  float fps;

  // methods

  private void SetTileInfo(BattleTile tile) {
    myText.text = tile
      ? string.Format("Current tile ({0}, {1})", tile.Coord.x, tile.Coord.y)
      : "";
  }

  private void SetFPSInfo() {
    timeleft -= Time.deltaTime;
    accum += Time.timeScale / Time.deltaTime;
    ++frames;

    if (timeleft <= 0.0) {
      fps = (accum / frames);
      timeleft = updateInterval;
      accum = 0.0f;
      frames = 0;
    }

    myText.text = fps.ToString("F2") + " FPS";
  }

  // init

  private void Init() {
    myText = GetComponent<Text>();
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

  private void Update() {
    SetFPSInfo();
  }

}

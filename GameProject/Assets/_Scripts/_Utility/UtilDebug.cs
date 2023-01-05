using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UtilDebug : MonoBehaviour
{

  // methods

  private void Restart() {
    if (!Input.GetKey(KeyCode.LeftControl)) return;
    if (!Input.GetKeyDown(KeyCode.R)) return;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
  }

  private void ToggleFullScreen() {
    if (!Input.GetKey(KeyCode.LeftControl)) return;
    if (!Input.GetKeyDown(KeyCode.W)) return;

    Screen.fullScreen = !Screen.fullScreen;
  }

  // signal methods

  private void PrintTileInfo(BattleTile tile) {
    if (!Input.GetKey(KeyCode.LeftControl)) return;
      if (tile) {
      print(string.Format("({0}, {1}) | {2} | {3}",
        tile.Coord.x,
        tile.Coord.y,
        tile.Unit != null ? "Has a unit" : "Has NO unit",
        tile.Unit && tile.Unit.IsHero ? "Hero" : "Not Hero"));
    } else {
      print("No tile.");
    }
  }

  // signal

  private void Subscribe() {
    BattleTileSelection.OnSelectTile += PrintTileInfo;
  }

  // base methods

  private void Awake() {
    Application.targetFrameRate = 60;
    QualitySettings.vSyncCount = 0;

    // Subscribe();
  }

  private void Update() {
    // Restart();
    ToggleFullScreen();
  }

}

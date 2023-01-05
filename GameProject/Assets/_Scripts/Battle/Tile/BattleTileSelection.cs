using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileSelection : Singleton<BattleTileSelection>
{

  private List<BattleTile> unhoverableTiles = new List<BattleTile>();

  private List<BattleTile> targetSelectedTiles = new List<BattleTile>();
  private List<BattleTile> targetHoveredTiles = new List<BattleTile>();

  public SkillScriptable pendingSkill { get; set; }

  private bool enableSelectTile = true;

  private BattleTile selectedTile;
  private BattleTile hoveredTile;

  public static DelegateTile OnSelectTile;
  public static DelegateTile OnHoverTile;

  // public methods

  public void SetPendingSkill(SkillScriptable skill) => pendingSkill = skill;

  public void SetAllTilesHoverable() {
    foreach (BattleTile tile in unhoverableTiles) {
      tile.SetHoverable();
    }
    unhoverableTiles.Clear();
  }

  public void SetAllTilesUnhoverable() {
    List<BattleTile> tileList = BattleTileManager.Instance.TileList;
    foreach (BattleTile tile in tileList) {
      tile.SetUnhoverable();
    }
    unhoverableTiles = new List<BattleTile>(tileList);
  }

  public void SetHoverableTiles(List<BattleTile> hoverables, bool excludeUnits = false) {
    SetAllTilesUnhoverable();
    foreach (BattleTile tile in hoverables) {
      if (excludeUnits && tile.Unit) continue;
      unhoverableTiles.Remove(tile);
      tile.SetHoverable();
    }
  }

  public void SetUnhoverableTiles(List<BattleTile> unhoverables) {
    SetAllTilesHoverable();
    foreach (BattleTile tile in unhoverables) {
      tile.SetUnhoverable();
    }
    unhoverableTiles = unhoverables;
  }

  public void ClearTargetHoveredTiles() {
    foreach (BattleTile targetHoveredTile in targetHoveredTiles) {
      targetHoveredTile.IsTargetHovered = false;
      targetHoveredTile.UpdateState();
    }
    targetHoveredTiles.Clear();
  }

  public void ClearTargetSelectedTiles() {
    foreach (BattleTile targetSelectedTile in targetSelectedTiles) {
      targetSelectedTile.IsTargetSelected = false;
      targetSelectedTile.UpdateState();
    }
    targetSelectedTiles.Clear();
  }

  public void ToggleEnableSelectTile(bool state) {
    enableSelectTile = state;
    if (!state) {
      SetHoveredTile(null);
    }
  }

  // methods

  private void HandleRayCheck() {
    if (!enableSelectTile) return;

    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    if (Physics.Raycast(ray, out hit)) {
      Transform objHit = hit.transform;
      BattleTile tileHit = objHit.GetComponent<BattleTile>();

      if (!unhoverableTiles.Contains(tileHit)) {
        SetHoveredTile(tileHit);
      } else {
        SetHoveredTile(null);
      }
    }
    else {
      SetHoveredTile(null);
    }
  }

  private void HandleSelectTile() {
    if (!enableSelectTile) return;
    if (!Input.GetMouseButtonUp(0)) return;
    if (BattleCameraRig.Instance.IsDragging) return;
    
    SetSelectedTile(
      HoveredTile && !HoveredTile.IsSelected ?
      HoveredTile :
      null);
  }

  private void SetSelectedTile(BattleTile tile) {
    if (HoveredTile) {
      HoveredTile.UpdateState();
    }
    if (SelectedTile) {
      SelectedTile.SetSelected(false);
    }

    // handle target selection
    ClearTargetSelectedTiles();

    if (tile != null) {
      targetSelectedTiles = new List<BattleTile>(targetHoveredTiles);
      foreach (BattleTile targetTile in targetSelectedTiles) {
        targetTile.IsTargetSelected = true;
        targetTile.UpdateState();
      }
    }

    // handle the new selected tile
    selectedTile = tile;
    OnSelectTile?.Invoke(tile);

    if (tile == null) return;
    tile.SetSelected(true);
  }

  private void SetHoveredTile(BattleTile tile) {
    if (tile == HoveredTile) return;

    if (HoveredTile) {
      HoveredTile.SetHovered(false);
    }

    // handle target selection
    SetTargetHoveredTile(tile);

    // handle the new hovered tile
    hoveredTile = tile;
    OnHoverTile?.Invoke(tile);

    if (tile == null) return;
    tile.SetHovered(true);
  }

  private void SetTargetHoveredTile(BattleTile tile) {
    if (!pendingSkill) return;

    ClearTargetHoveredTiles();

    if (tile == null) return;

    List<BattleTile> targetTileList = BattleTileManager.Instance.GetSkillScopeTiles(pendingSkill.id, tile);

    foreach (BattleTile targetTile in targetTileList) {
      targetTile.IsTargetHovered = true;
      targetTile.UpdateState();
      targetHoveredTiles.Add(targetTile);
    }
  }

  // base methods

  private void Update() {
    HandleRayCheck();
    HandleSelectTile();
  }

  // get set

  public BattleTile SelectedTile {
    get { return selectedTile; }
    set { SetSelectedTile(value); }
  }

  public BattleTile HoveredTile {
    get { return hoveredTile; }
    set { SetHoveredTile(value); }
  }

}

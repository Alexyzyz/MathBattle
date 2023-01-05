using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTile : MonoBehaviour
{

  private enum TileState {
    Idle,
    Unhoverable,
  }

  private List<Vector2Int> NEIGHBOR_POS_OFFSETS_ODD = new List<Vector2Int> {
    new Vector2Int( 0,  1), // 0
    new Vector2Int( 1,  0), // 1
    new Vector2Int( 0, -1), // 2
    new Vector2Int(-1, -1), // 3
    new Vector2Int(-1,  0), // 4
    new Vector2Int(-1,  1), // 5
  };
  private List<Vector2Int> NEIGHBOR_POS_OFFSETS_EVEN = new List<Vector2Int> {
    new Vector2Int( 1,  1), // 0
    new Vector2Int( 1,  0), // 1
    new Vector2Int( 1, -1), // 2
    new Vector2Int( 0, -1), // 3
    new Vector2Int(-1,  0), // 4
    new Vector2Int( 0,  1), // 5
  };

  private MeshRenderer myMeshRenderer;
  private SpriteRenderer myOutline;

  public BattleUnitBase Unit { get; set; }
  public Vector2Int Coord { get; set; }
  public Vector3 WorldPos { get; set; }
  public bool HasSelectedHero { get; set; }

  public bool IsSelected { get; private set; }
  public bool IsHovered { get; private set; }

  public bool IsTargetSelected { get; set; }
  public bool IsTargetHovered { get; set; }

  private TileState state;

  private float ratioUnhoverable = 0.6f;

  private float ratioSelect = 1.7f;
  private float ratioHover = 1.5f;
  private float ratioTargetSelect = 1.3f;

  // public methods

  public void SetHoverable() {
    state = TileState.Idle;
    UpdateState();
  }

  public void SetUnhoverable() {
    state = TileState.Unhoverable;
    UpdateState();
  }

  public bool IsNeighbor(BattleTile tile) {
    if (tile == null) return false;

    List<Vector2Int> neighborPosOffsets = GetNeighborPosOffsets();

    foreach (Vector2 offset in neighborPosOffsets) {
      if (tile.Coord == Coord + offset) return true;
    }
    return false;
  }


  /// <summary>
  /// Gets one of this tile's neighbor. Index starts from 0 (northeast) and goes clockwise until 5 (northwest).
  /// </summary>
  /// <param name="i"></param>
  /// <returns></returns>
  public BattleTile GetNeighbor(int i) {
    if (i < 0) {
      print(string.Format("LOG: Invalid index {0}", i));
      return null;
    }
    List<Vector2Int> neighborPosOffsets = GetNeighborPosOffsets();
    return BattleTileManager.Instance.GetTile(Coord + neighborPosOffsets[i % 6]);
  }

  /// <summary>
  /// Gets the neighbor index from this neighbor tile.
  /// </summary>
  /// <param name="i"></param>
  /// <returns></returns>
  public int GetNeighborIndex(Vector2Int coord) {
    List<Vector2Int> neighborPosOffsets = GetNeighborPosOffsets();
    return neighborPosOffsets.IndexOf(coord - Coord);
  }

  public List<BattleTile> GetNeighbors() {
    List<BattleTile> neighborList = new List<BattleTile>();
    List<Vector2Int> neighborPosOffsets = GetNeighborPosOffsets();

    foreach (Vector2Int offset in neighborPosOffsets) {
      BattleTile neighbor = BattleTileManager.Instance.GetTile(Coord + offset);
      if (neighbor) neighborList.Add(neighbor);
    }
    return neighborList;
  }

  public void SetSelected(bool selected) {
    IsSelected = selected;
    UpdateState();

    if (Unit && !Unit.IsCurrentlyBeingAttacked) {
      Unit.SetOverheadInfoAlpha(selected ? 1f : 0f);
    }
  }
  public void SetHovered(bool hovered) {
    IsHovered = hovered;
    UpdateState();

    if (IsSelected == false && Unit && !Unit.IsCurrentlyBeingAttacked) {
      Unit.SetOverheadInfoAlpha(hovered ? 0.6f : 0f);
    }
  }

  public void UpdateState() {
    UpdateOutline();
    UpdateTile();
  }

  // methods

  private void UpdateTile() {
    if (state == TileState.Unhoverable) {
      // darken color to show it's unhoverable
      // overrides everything else
      myMeshRenderer.material.color = GetBaseTileColor() * ratioUnhoverable;
    }
    else {
      // normal case
      float ratio = 1;
      if (IsSelected) { ratio = ratioSelect; } else
      if (IsHovered || IsTargetHovered) { ratio = ratioHover; } else
      if (IsTargetSelected) { ratio = ratioTargetSelect; }
      myMeshRenderer.material.color = GetBaseTileColor() * ratio;
    }
  }

  private void UpdateOutline() {
    // handle outline state
    float outlineAlpha;
    if (IsSelected) {
      outlineAlpha = 1;

      if (animSelectedOutline == null) {
        animSelectedOutline = AnimateSelectedOutline();
        StartCoroutine(animSelectedOutline);
      }
    }
    else {
      animSelectedOutline = null;
      if (IsHovered) {
        outlineAlpha = 0.5f;
      } else {
        outlineAlpha = Unit ? 0 : 10f / 255f;
      }
    }
    myOutline.color = new Color(0, 0, 0, outlineAlpha);
  }

  // init

  private void Init() {
    myMeshRenderer = GetComponent<MeshRenderer>();
    myMeshRenderer.material.color = GetBaseTileColor();

    myOutline = transform.Find("Outline").GetComponent<SpriteRenderer>();
  }

  // util

  private Color GetBaseTileColor() {
    if (Unit) {
      return Unit.IsHero ? UtilColor.TileHero : UtilColor.TileEnemy;
    }
    return new Color32(166, 144, 90, 255);
  }

  private List<Vector2Int> GetNeighborPosOffsets() {
    return Coord.y % 2 == 0 ?
      NEIGHBOR_POS_OFFSETS_EVEN :
      NEIGHBOR_POS_OFFSETS_ODD;
  }

  // base methods

  private void Awake() {
    Init();
  }

  private void Start() {
    UpdateState();
  }



  // animations

  private IEnumerator animSelectedOutline;

  private IEnumerator AnimateSelectedOutline() {
    float animSelectedOutline_SineCounter = 0;
    while (IsSelected) {
      float pad = 0.03f;
      float outlineScale = pad + pad * Mathf.Sin(animSelectedOutline_SineCounter);
      myOutline.transform.localScale = 2f * new Vector3(1, 1, 0) + outlineScale * new Vector3(1, 1, 0);
      animSelectedOutline_SineCounter += 7 * Time.deltaTime;
      yield return null;
    }
    myOutline.transform.localScale = 2 * Vector3.one;
  }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileManager : Singleton<BattleTileManager>
{

  private const int MAP_SIZE = 12;

  [SerializeField] private BattleTile tilePrefab;
  [SerializeField] private Transform tileParent;

  public List<BattleTile> TileList { get; private set; } = new List<BattleTile>();
  private BattleTile[,] tileMap = new BattleTile[MAP_SIZE, MAP_SIZE];

  // public methods

  public List<BattleTile> GetSkillScopeTiles(string skillName, BattleTile refTile) {
    List<BattleTile> tileList = new List<BattleTile>();

    if (refTile == null) return tileList;

    if (skillName == "forward wide") {
      // get 3 tiles in a swipe in front
      int mainTargetTileIndex = SelectedHero.Tile.GetNeighborIndex(refTile.Coord);
      List<int> range = new List<int> { -1, 0, 1 };
      foreach (int i in range) {
        BattleTile tempTile = SelectedHero.Tile.GetNeighbor(UtilMath.WrapValue(mainTargetTileIndex, i, 0, 5));
        tileList.Add(tempTile);
      }
    }
    else
    if (skillName == "range wide") {
      // get 3-length radial on target
      tileList = GetRadialCoverage(refTile, 2);
    }
    else {
      // default
      tileList.Add(refTile);
    }
    return tileList;
  }

  public List<BattleTile> GetSkillHoverableTiles(string skillName) {
    List<BattleTile> tileList = new List<BattleTile>();

    if (skillName == "switcheroo") {
      // 8-length radius
      tileList = GetRadialCoverage(SelectedHero.Tile, 8);
    } else
    if (skillName == "range single") {
      // 7-length radius
      tileList = GetRadialCoverage(SelectedHero.Tile, 7);
    }
    else
    if (skillName == "range wide") {
      // 6-length radius
      tileList = GetRadialCoverage(SelectedHero.Tile, 6, true);
    }
    else {
      // default
      tileList = SelectedTile.GetNeighbors();
    }
    return tileList;
  }

  public BattleTile GetTile(Vector2Int coord) {
    int yy = coord.y;
    int xx = coord.x;
    if (xx >= 0 && xx < MAP_SIZE &&
        yy >= 0 && yy < MAP_SIZE &&
        tileMap[yy, xx] != null) {
      return tileMap[yy, xx];
    }
    return null;
  }

  // methods

  private void GenerateMap() {
    // the hexagon's width across its flat edges        :: sqrt(3)
    // the hexagon's height across its pointy edges     :: 2
    // the hexagon's height excluding its pointy edges  :: 1.5

    float tilePrefabScale = tilePrefab.transform.localScale.x;
    float tileWidth = Mathf.Sqrt(3) * tilePrefabScale;
    float tileHeight = 1.5f * tilePrefabScale;

    for (int yy = 0; yy < MAP_SIZE; yy++) {
      float xOffset = (yy % 2 == 0) ? (0.5f * tileWidth) : 0;
      for (int xx = 0; xx < MAP_SIZE; xx++) {
        float xxx = xx - MAP_SIZE / 2;
        float yyy = yy - MAP_SIZE / 2;
        if (Mathf.Sqrt(xxx * xxx + yyy * yyy) > MAP_SIZE / 2) continue;

        Vector3 tileWorldPos = new Vector3(tileWidth * xx + xOffset, 0, tileHeight * yy);

        BattleTile newTile = Instantiate(
          tilePrefab,
          tileWorldPos,
          tilePrefab.transform.rotation,
          tileParent);
        
        newTile.Coord = new Vector2Int(xx, yy);
        newTile.WorldPos = tileWorldPos;


        TileList.Add(newTile);
        tileMap[yy, xx] = newTile;
      }
    }

    BattleUnitManager.Instance.SpawnHero(new Vector2Int(6, 6), new UnitStats(200, 4, 1));
    BattleUnitManager.Instance.SpawnEnemy(new Vector2Int(5, 4), new UnitStats(30, 4, 1));
  }

  // util

  private List<BattleTile> GetRadialCoverage(BattleTile originTile, int radius, bool alwaysIncludeOrigin = false) {
    List<BattleTile> nextLayer = originTile.GetNeighbors();

    if (alwaysIncludeOrigin || (originTile.Unit == null || !originTile.Unit.IsHero))
      nextLayer.Add(originTile);

    List<BattleTile> currLayer;
    List<BattleTile> total = nextLayer;

    int i = radius - 2;
    while (i > 0) {
      currLayer = new List<BattleTile>(nextLayer);
      foreach (BattleTile neighbor in currLayer) {
        List<BattleTile> newNeighbors = neighbor.GetNeighbors();

        foreach (BattleTile newNeighbor in newNeighbors) {
          bool hasHeroUnit = newNeighbor.Unit == null || !newNeighbor.Unit.IsHero;
          if (hasHeroUnit && !total.Contains(newNeighbor)) {
            total.Add(newNeighbor);
            nextLayer.Add(newNeighbor);
          }
        }

      }
      i--;
    }
    return total;
  }

  private BattleUnitBase SelectedHero {
    get {
      return BattleUnitManager.Instance.SelectedHero;
    }
  }

  private BattleTile SelectedTile {
    get {
      return BattleTileSelection.Instance.SelectedTile;
    }
  }

  // base methods

  void Start() {
    GenerateMap();
  }

}

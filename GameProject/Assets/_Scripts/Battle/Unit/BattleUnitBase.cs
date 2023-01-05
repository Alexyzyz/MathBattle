using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class BattleUnitBase : MonoBehaviour
{

  [SerializeField] protected BattleCanvasUnitOverhead overheadPrefab;
  [SerializeField] protected BattleUnitBaseMoveShadow moveShadowPrefab;

  [SerializeField]
  protected Sprite mySprite;

  protected Transform moveShadowParent;

  protected Animator myAnimator;
  protected SpriteRenderer mySpriteRenderer;

  public List<SkillScriptable> SkillList { get; set; } = new List<SkillScriptable>();
  public List<Item> ItemList { get; set; } = new List<Item>();
  public BattleCanvasUnitOverhead Overhead { get; protected set; }

  public Vector3 WorldPos {
    get {
      return transform.position;
    }
  }
  public Vector2Int Coord {
    get {
      return Tile != null ? Tile.Coord : Vector2Int.zero;
    }
  }

  public BattleTile Tile { get; protected set; }
  public virtual bool IsHero { get; }

  public UnitStats Stats { get; set; }

  public bool AlwaysFaceCamera { get; set; } = true;

  public bool IsCurrentlyBeingAttacked { get; set; }

  // public methods

  public void OnDefeatedAnimationFinish() {
    Destroy(Overhead.gameObject);
    Destroy(gameObject);
  }

  public void StartDefeatedAnimation() {
    myAnimator.SetTrigger("Defeated");
  }

  public virtual void HandleDefeat() {
    // BattleUnitManager.Instance.RemoveUnit(this);
    BattleCanvasUnitOverheadContainer.Instance.Remove(Overhead);
  }

  /// <summary>
  /// Returns true if move was successful. False otherwise.
  /// </summary>
  public bool MoveToCoord(Vector2Int coord, bool playAnimation = true) {
    BattleTile newTile = BattleTileManager.Instance.GetTile(coord);
    if (!newTile) {
      print(string.Format("ERROR: Attempt to move unit to invalid tile ({0}, {1}).", newTile.Coord.x, newTile.Coord.y));
      return false;
    }
    if (newTile.Unit) {
      print(string.Format("ERROR: Attempt to move unit to tile with pre-existing unit ({0}, {1}).", newTile.Coord.x, newTile.Coord.y));
      return false;
    }
    
    if (playAnimation) {
      PlayAnimateMove(newTile);
    } else {
      transform.position = newTile.transform.position;
    }

    if (Tile) {
      Tile.Unit = null;
      Tile.UpdateState();
    }

    Tile = newTile;
    newTile.Unit = this;
    newTile.UpdateState();

    if (playAnimation) {
      GameSoundManager.Instance.PlaySound(GameSound.BattleMove);
    }

    return true;
  }

  public void SwitchToCoord(Vector2Int coord, bool playAnimation = true) {
    BattleTile newTile = BattleTileManager.Instance.GetTile(coord);
    if (!newTile) {
      print(string.Format("ERROR: Attempt to switch unit with an invalid one at ({0}, {1}).", newTile.Coord.x, newTile.Coord.y));
      return;
    }

    if (playAnimation) {
      PlayAnimateMove(newTile);
    }
    else {
      transform.position = newTile.transform.position;
    }

    if (newTile.Unit != null) {
      // if new tile has a unit
      if (playAnimation) {
        newTile.Unit.PlayAnimateMove(Tile);
      } else {
        newTile.Unit.transform.position = Tile.transform.position;
      }
      newTile.Unit.Tile = Tile;
    }

    if (newTile == BattleTileSelection.Instance.SelectedTile) {
      BattleTileSelection.Instance.SelectedTile = newTile;
    }

    Tile.Unit = newTile.Unit;
    Tile.UpdateState();

    Tile = newTile;
    newTile.Unit = this;
    newTile.UpdateState();

  }

  public void PlayAnimateMove(BattleTile tile) {
    StartCoroutine(AnimateMove(tile.transform.position));
  }

  public void SetToAttack(List<BattleUnitBase> targetList) {
    // these will be reset after the animation ends
    AlwaysFaceCamera = false;

    int randomTargetUnitIndex = (int)Mathf.Floor(targetList.Count / 2);

    transform.LookAt(targetList[randomTargetUnitIndex].transform);
    transform.Rotate(Vector3.up, -90);

    foreach (BattleUnitBase target in targetList) {
      target.AlwaysFaceCamera = false;
      target.transform.LookAt(transform);
      target.transform.Rotate(Vector3.up, -90);

      target.IsCurrentlyBeingAttacked = true;
    }

    myAnimator.SetTrigger("Attack");
  }

  public void SetToDefend() {
    myAnimator.SetTrigger("Defend");
  }

  public void TakeDamage(int damage) {
    damage = damage > Stats.HP ? Stats.HP : damage;

    Stats.HP -= damage;
    Overhead.Info.SetHP(Stats.HP, Stats.maxHP);

    if (Stats.HP == 0) {
      // is defeated
      HandleDefeat();
    }

    // The following events occur AFTER the HP lag bar finishes its animation:
    // 1  IsCurrentlyBeingAttacked is set to false
    // 2  HandleDefeat can called
  }

  public void SetStats(UnitStats stats) {
    Stats = stats;
    mySpriteRenderer.sprite = mySprite;
    
    Overhead.SetUnit(this);
  }

  public void SetOverheadInfoAlpha(float alpha) {
    if (Overhead && Overhead.Info)
      Overhead.Info.SetAlpha(alpha);
  }

  // methods

  protected void FaceCamera() {
    if (!AlwaysFaceCamera) return;
    Vector3 lookAtPos = Camera.main.transform.position;
    lookAtPos.y = transform.position.y;
    transform.LookAt(lookAtPos);
    transform.Rotate(Vector3.up, 180);
  }

  // init

  protected void Init() {
    myAnimator = transform.Find("Rig").GetComponent<Animator>();
    mySpriteRenderer = transform.Find("Rig").GetComponent<SpriteRenderer>();

    moveShadowParent = GameObject.Find("Environment/UnitShadowParent").transform;

    mySpriteRenderer.sprite = mySprite;

    Transform overheadInfoParent = GameObject.Find("Canvases/MainCanvas/BattleUnitOverheadInfoParent").transform;
    Overhead = Instantiate(overheadPrefab, Vector3.zero, Quaternion.identity, overheadInfoParent);
    BattleCanvasUnitOverheadContainer.Instance.Add(Overhead);
  }

  // base methods

  protected void Awake() {
    Init();
  }

  protected void Update() {
    FaceCamera();
  }

  // get set

  public virtual int HP {
    get { return Stats.HP; }
    set {
      int newValue = Mathf.Clamp(value, 0, Stats.maxHP);
      Stats.HP = newValue;
    }
  }

  public virtual int AP {
    get { return Stats.AP; }
    set {
      int newValue = Mathf.Clamp(value, 0, Stats.maxAP);
      Stats.AP = newValue;
    }
  }

  // animations

  protected IEnumerator AnimateMove(Vector3 targetPos) {
    AlwaysFaceCamera = false;

    Vector3 currPos = transform.position;

    float val = 0;
    while (val < 0.99f) {
      UpdateProps();
      val = Mathf.Lerp(val, 1, 0.4f);
      yield return null;
    }
    val = 1;
    UpdateProps();

    AlwaysFaceCamera = true;

    void UpdateProps() {
      Color start = new Color32(61, 235, 232, 255);
      Color end = new Color32(61, 113, 235, 255);
      Color shadowColor = Color.Lerp(start, end, val);

      BattleUnitBaseMoveShadow shadow = Instantiate(moveShadowPrefab, transform.position, transform.rotation, moveShadowParent);
      shadow.SetData(mySprite, shadowColor);

      transform.position = Vector3.Lerp(currPos, targetPos, val);
    }
  }

}

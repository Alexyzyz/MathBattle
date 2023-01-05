using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameSound
{
  BattleAnswerCorrect,
  BattleAnswerIncorrect,
  BattleAttack,
  BattleMove,
  BattleFinish,
}

public class GameSoundManager : Singleton<GameSoundManager>
{

  private AudioSource soundPlayer;
  
  [SerializeField] private AudioClip sound_Battle_AnswerCorrect;
  [SerializeField] private AudioClip sound_Battle_AnswerIncorrect;
  [SerializeField] private AudioClip sound_Battle_Attack;
  [SerializeField] private AudioClip sound_Battle_Move;
  [SerializeField] private AudioClip sound_Battle_Finish;

  // public methods

  public void PlaySound(GameSound id) {
    AudioClip tobePlayed = GetClip(id);
    soundPlayer.PlayOneShot(tobePlayed);
  }

  // init

  private void Init() {
    soundPlayer = GetComponent<AudioSource>();
  }

  // util

  private AudioClip GetClip(GameSound id) {
    switch (id) {
      case GameSound.BattleAnswerCorrect: return sound_Battle_AnswerCorrect;
      case GameSound.BattleAnswerIncorrect: return sound_Battle_AnswerIncorrect;
      case GameSound.BattleAttack: return sound_Battle_Attack;
      case GameSound.BattleFinish: return sound_Battle_Finish;
      case GameSound.BattleMove: return sound_Battle_Move;

      default: return sound_Battle_AnswerCorrect;
    }
  }

  // base methods

  protected override void Awake() {
    base.Awake();
    Init();
  }

}

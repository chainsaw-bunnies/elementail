using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
  AudioClip CurrentMusic;
  int SourceIndex;
  AudioSource[] Sources;

  void Awake()
  {
    var pool = new GameObject("Music Pool");
    pool.transform.parent = gameObject.transform;

    CurrentMusic = null;
    SourceIndex = 0;
    Sources = new AudioSource[2];
    Sources[0] = pool.AddComponent<AudioSource>();
    Sources[1] = pool.AddComponent<AudioSource>();
  }

  public void Play(AudioClip clip)
  {
    if (CurrentMusic == clip) { return; }
    CurrentMusic = clip;

    var toFadeOut = Sources[SourceIndex];
    toFadeOut.DOFade(0f, 1f).OnComplete(() => toFadeOut.Stop());

    SourceIndex = SourceIndex == 0 ? 1 : 0;

    var toFadeIn = Sources[SourceIndex];
    toFadeIn.volume = 0f;
    toFadeIn.clip = clip;
    toFadeIn.loop = true;
    toFadeIn.DOFade(1f, 1f).OnStart(() => toFadeIn.Play());
  }
}

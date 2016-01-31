﻿using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
  public Sprite PlainSprite;
  public Sprite[] DecoratedSprites;
  public Sprite[] NoGlowRuneSprites;
  public Sprite[] YellowGlowRuneSprites;
  public Sprite[] RedGlowRuneSprites;
  public float DangerTime;

  int SpriteIndex;

  public SpriteRenderer GroundRenderer;

  public SpriteRenderer RuneRenderer;
  public SpriteRenderer RuneGlowRenderer;

  public bool Runed { get; private set; }
  public bool Dangerous { get; private set; }

  [HideInInspector]
  public bool IsDecorated;

  void Start()
  {
    if (IsDecorated)
    {
      GroundRenderer.sprite = DecoratedSprites[Random.Range(0, DecoratedSprites.Length)];
    }
    else
    {
      GroundRenderer.sprite = PlainSprite;
    }

    Dangerous = false;
    Runed = false;

    SpriteIndex = Random.Range(0, NoGlowRuneSprites.Length);
    RuneRenderer.sprite = NoGlowRuneSprites[SpriteIndex];
    RuneRenderer.enabled = false;
    RuneGlowRenderer.sprite = YellowGlowRuneSprites[SpriteIndex];
    RuneGlowRenderer.enabled = false;
    RuneGlowRenderer.color = new Color(1f, 1f, 1f, 0f);
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.tag != "Player") { return; }
    other.gameObject.GetComponent<Player>().EnterTile(this);
  }

  void OnTriggerExit(Collider other)
  {
    if (other.tag != "Player") { return; }
    other.gameObject.GetComponent<Player>().ExitTile(this);
  }

  public void ActivateRune()
  {
    Runed = true;
    RuneRenderer.enabled = true;
    RuneGlowRenderer.enabled = true;
    gameObject.AddComponent<BecomeDangerous>().Ground = this;
  }

  public void DangerifyRune()
  {
    Dangerous = true;
    RuneGlowRenderer.sprite = RedGlowRuneSprites[SpriteIndex];
  }
}

public class BecomeDangerous : MonoBehaviour
{
  public Ground Ground;
  float StartTime;

  void Start()
  {
    StartTime = Time.time;
  }

  void Update()
  {
    var elapsed = Time.time - StartTime;
    var elapsedPct = elapsed / Ground.DangerTime;

    Ground.RuneGlowRenderer.color = new Color(1f, 1f, 1f, elapsedPct);

    // Destroy this script.
    if ((elapsedPct) > 1f)
    {
      Ground.DangerifyRune();
      Destroy(this);
    }
  }
}

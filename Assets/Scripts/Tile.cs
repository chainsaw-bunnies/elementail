using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour
{
  public bool IsActivated   { get; private set;}
  public bool IsDangerous   { get; private set; }
  public bool IsObstacle    { get; private set; }
  public bool IsPickup      { get; private set; }
  public bool IsRitualPoint { get; private set; }
  public bool IsRuned       { get; private set; }
  Action Action;
  List<SpriteRenderer> Renderers;

  void Awake()
  {
    IsDangerous = false;
    IsObstacle = false;
    IsRuned = false;
    Action = null;
    Renderers = new List<SpriteRenderer>();
  }

  public SpriteRenderer AddSprite(Sprite sprite)
  {
    var renderer = new GameObject("Renderer").AddComponent<SpriteRenderer>();
    renderer.sprite = sprite;
    renderer.transform.SetParent(transform, false);
    Renderers.Add(renderer);

    for (int i = 0; i < Renderers.Count; i++)
    {
      Renderers[i].sortingOrder = i;
    }

    return renderer;
  }

  public void AddObstacle(Sprite sprite)
  {
    IsObstacle = true;
    AddSprite(sprite);

    name = "Obstacle";
  }

  public void AddPickup(Pickup pickup)
  {
    IsPickup = true;

    var renderer = AddSprite(pickup.Sprite);
    Action = () =>
    {
      IsPickup = false;
      GameStatus.Root.SoundPlayer.Play(pickup.Sound);
      GameStatus.Score += pickup.Score;
      renderer.enabled = false;
    };

    name = pickup.Name;
  }

  public void AddRitualPoint(RuntimeAnimatorController controller)
  {
    IsRitualPoint = true;

    var renderer = AddSprite(null);
    var animator = renderer.gameObject.AddComponent<Animator>();
    animator.runtimeAnimatorController = controller;

    Action = () =>
    {
      IsActivated = true;
      GameStatus.ActivateRitualPoint();
      animator.Play("RitualActivating");

      if (GameStatus.RitualPointsRemaining == 0)
      {
        GameStatus.Root.SoundPlayer.Play(GameStatus.Root.SoundBank.Happy);
        GameStatus.Root.SoundPlayer.Play(GameStatus.Root.SoundBank.Win);
        GameStatus.Root.Player.enabled = false;
        GameStatus.LevelsCompleted++;

        var portalPlain = AddSprite(GameStatus.Root.SpriteBank.PortalPlain);
        var portalGlow = AddSprite(GameStatus.Root.SpriteBank.PortalGlow);

        var rotationZ = UnityEngine.Random.Range(0f, 360f);
        var rotationSpeed = 100f;

        var scale = Vector3.one;
        var scaleSpeed = 4f;

        portalGlow.color = new Color(portalGlow.color.r, portalGlow.color.g, portalGlow.color.b, 0f);
        portalGlow.DOFade(1f, 2f).OnUpdate(() =>
        {
          rotationZ += rotationSpeed * Time.deltaTime;
          scale += (Vector3.one * scaleSpeed) * Time.deltaTime;
          foreach (var r in new[] { portalPlain, portalGlow })
          {
            r.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
            r.transform.localScale = scale;
          }
        }).OnComplete(() =>
        {
          GameStatus.Root.ExitLevel("MainGame");
        });
      }
    };

    name = "Ritual Point";
  }

  public void Activate()
  {
    Action();
  }

  public void Enter()
  {
    if (IsDangerous)
    {
      GameStatus.Root.SoundPlayer.Play(GameStatus.Root.SoundBank.Damage);
      GameStatus.Root.Player.enabled = false;
      GameStatus.Root.ExitLevel("GameOver");
    }

    if (IsPickup)
    {
      Action();
    }
  }

  public void Leave()
  {
    if (!IsRuned)
    {
      IsRuned = true;

      var runeSet = GameStatus.Root.RuneSets.RandomElement();
      var rotation = UnityEngine.Random.Range(0f, 360f);

      var plain = AddSprite(runeSet.NoGlow);
      plain.transform.rotation = Quaternion.Euler(0f, 0f, rotation);

      var glow = AddSprite(runeSet.Yellow);
      glow.transform.rotation = Quaternion.Euler(0f, 0f, rotation);

      glow.color = new Color(1f, 1f, 1f, 0f);
      glow.DOFade(1f, GameStatus.Root.RuneTimeUntilDangerous).OnComplete(() =>
      {
        IsDangerous = true;
        glow.sprite = runeSet.Red;
      });

      GameStatus.PlaceRune();
    }
  }
}

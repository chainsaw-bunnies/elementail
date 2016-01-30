using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
  public Sprite[] WallSprites;

  public SpriteRenderer GroundRenderer;
  public SpriteRenderer WallRendererer;


  void Start ()
  {
    WallRendererer.sprite = WallSprites[UnityEngine.Random.Range(0, WallSprites.Length)];
  }
}

using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
  public Sprite OutermostWallSprite;
  public Sprite[] RegularWallSprites;

  public SpriteRenderer GroundRenderer;
  public SpriteRenderer WallRendererer;

  [HideInInspector]
  public bool IsOutermostWall;

  void Start ()
  {
    if (IsOutermostWall)
    {
      WallRendererer.sprite = OutermostWallSprite;
    }
    else
    {
      WallRendererer.sprite = RegularWallSprites[UnityEngine.Random.Range(0, RegularWallSprites.Length)];
    }
  }
}

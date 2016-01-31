using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour
{
  public bool Activated { get; private set; }
  public Sprite ActivatedSprite;
  public Sprite DeactivatedSprite;
  public SpriteRenderer SpriteRenderer;

  void OnTriggerEnter(Collider other)
  {
    if (other.tag != "Player") { return; }
    other.gameObject.GetComponent<Player>().Hop(this);
  }

  void OnTriggerExit(Collider other)
  {
    if (other.tag != "Player") { return; }

    if (!Activated)
    {
      Activated = true;
      SpriteRenderer.sprite = ActivatedSprite;
      ScoreBox.LeavesRemaining--;
    }

    other.gameObject.GetComponent<Player>().Unhop(this);
  }

}

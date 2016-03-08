using UnityEngine;
using System.Collections;

public class Leaf : MonoBehaviour
{
  
	public Animator Animator;
  public bool Activated { get; private set; }
  public Sprite ActivatedSprite;
  public Sprite DeactivatedSprite;
  public SpriteRenderer SpriteRenderer;

	void Start()
	{
		Animator.Play ("RitualUnactivated");
	}

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
     // SpriteRenderer.sprite = ActivatedSprite;
      ScoreBox.LeavesRemaining--;
      ScoreBox.Score += 100 * ScoreBox.Level;
			Animator.SetInteger ("State", 1);
			Animator.Play ("RitualActivating");
			Animator.SetInteger ("State", 2);
    }


    other.gameObject.GetComponent<Player>().Unhop(this);
  }

}

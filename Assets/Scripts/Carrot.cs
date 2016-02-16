using UnityEngine;
using System.Collections;

public class Carrot : MonoBehaviour
{
  public SpriteRenderer Renderer;
  public Sprite Normal;
  public Sprite Super;
  public AudioSource Sound;

  [HideInInspector]
  public bool IsSuper;

  bool PickedUp;

	void Start()
  {
    IsSuper = Random.value < 0.1f;
    PickedUp = false;
    Renderer.sprite = IsSuper ? Super : Normal;
	}

  void OnTriggerEnter(Collider other)
  {
    if (other.tag != "Player" || PickedUp) { return; }

    Sound.Play();
    Renderer.enabled = false;
    ScoreBox.Score += IsSuper ? 500 : 50;
  }
}

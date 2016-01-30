using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
  public Sprite[] BaseSprites;
  public Sprite[] RuneSprites;
  public SpriteRenderer GroundRenderer;
  public SpriteRenderer DustRenderer;
  public bool Runed
  {
    get; private set;
  }

  void Start()
  {
    GroundRenderer.sprite = BaseSprites[Random.Range(0, BaseSprites.Length)];

    Runed = false;
    DustRenderer.sprite = RuneSprites[Random.Range(0, RuneSprites.Length)];
    DustRenderer.enabled = false;
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.tag != "Player") { return; }
    other.gameObject.GetComponent<Player>().EnterTile(this);
  }

  void OnTriggerLeave(Collider other)
  {
    if (other.tag != "Player") { return; }
    other.gameObject.GetComponent<Player>().LeaveTile(this);
  }

  public void ActivateRune()
  {
    Runed = true;
    DustRenderer.enabled = true;
  }
}

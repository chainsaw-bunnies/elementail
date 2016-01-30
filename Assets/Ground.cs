using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
  const float DustedMinimumDistance = 1.5f;

  public Sprite[] BaseSprites;
  public Sprite DustSprite;
  public SpriteRenderer Base;
  public SpriteRenderer Dust; 

  void Start()
  {
    Base.sprite = BaseSprites[Random.Range(0, BaseSprites.Length)];
    Dust.sprite = DustSprite;
    Dust.enabled = false;
  }

  void OnTriggerStay(Collider other)
  {
    var distance = Vector3.Distance(other.transform.position, transform.position);
    if (other.tag == "Player" && !Dust.enabled && distance < DustedMinimumDistance)
    {
      Dust.enabled = true;
    }
  }
}

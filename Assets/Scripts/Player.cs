using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
  int LeftRunesCount;
  float Speed;
  public float MaxSpeed = 15f;
  public SpriteRenderer SpriteRenderer;

  // Allow player to slightly overlap dangerous tiles since otherwise the player might be overlapping a tile
  // however our art is such that it will still look like the art is not touching the player.
  const float DangerousTileTolerance = 2.75f; 

  CharacterController CharacterController;
  HashSet<Ground> CurrentGrounds;
  KeyCode LastKeyUp;

  void Start()
  {
    CharacterController = GetComponent<CharacterController>();
    CurrentGrounds = new HashSet<Ground>();
    MoveDirection = Vector3.zero;
  }

  void FixedUpdate()
  {
    Speed = MaxSpeed;

    var moveDirection = Vector3.zero;
    switch (LastKeyUp)
    {
      case KeyCode.UpArrow:    moveDirection = new Vector3(0f, 1f, 0f);  break;
      case KeyCode.DownArrow:  moveDirection = new Vector3(0f, -1f, 0f); break;
      case KeyCode.RightArrow: moveDirection = new Vector3(1f, 0f, 0f);  break;
      case KeyCode.LeftArrow:  moveDirection = new Vector3(-1f, 0f, 0f); break;
    }

    // Update the direction you are facing.
    SpriteRenderer.flipX = moveDirection.x > 0;

    CharacterController.Move(moveDirection * Speed * Time.fixedDeltaTime);
  }

  void Update()
  {
    if (Input.GetKeyUp(KeyCode.Escape))
    {
      SceneManager.LoadScene("MainMenu");
    }

    var keys = new[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    foreach (var key in keys)
    {
      if (Input.GetKeyUp(key))
      {
        LastKeyUp = key;
      }
    }
  }

  #region Runes

  public void EnterTile(Ground ground)
  {
    CurrentGrounds.Add(ground);
  }

  public void ExitTile(Ground ground)
  {
    CurrentGrounds.Remove(ground);
  }

  void LateUpdate()
  {
    if (Mathf.Approximately(0f, MoveDirection.magnitude))
    {
      return;
    }

    Ground closest = null;
    float bestDistance = Mathf.Infinity;

    // Only affect the ground of the tile you overlap with the most.
    foreach (var ground in CurrentGrounds)
    {
      var dist = Vector3.Distance(transform.position, ground.transform.position);

      // Lose
      if (ground.Dangerous && dist < DangerousTileTolerance)
      {
        SceneManager.LoadScene("GameOver");
        return;
      }

      if (dist < bestDistance)
      {
        closest = ground;
        bestDistance = dist;
      }
    }

    if (closest != null && !closest.Runed)
    {
      LeftRunesCount++;
      closest.ActivateRune();
    }
  }

  #endregion
}

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

  // Allow player to slightly overlap dangerous tiles since otherwise the player might be overlapping a tile
  // however our art is such that it will still look like the art is not touching the player.
  const float DangerousTileTolerance = 2.75f; 

  CharacterController CharacterController;
  HashSet<Ground> CurrentGrounds;
  Vector3 MoveDirection;

  void Start()
  {
    CharacterController = GetComponent<CharacterController>();
    CurrentGrounds = new HashSet<Ground>();
    MoveDirection = Vector3.zero;
  }

  void FixedUpdate()
  {
    Speed = MaxSpeed;

    CharacterController.Move(MoveDirection * Speed * Time.fixedDeltaTime);
  }

  void Update()
  {
    var keys = new[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    foreach (var key in keys)
    {
      if (Input.GetKeyUp(key))
      {
        MoveDirection = KeyCodeToDirection(key);
      }
    }

    if (Input.GetKeyUp(KeyCode.Escape))
    {
      Application.Quit();
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

  static Vector3 KeyCodeToDirection(KeyCode code)
  {
    switch (code)
    {
      case KeyCode.UpArrow:  return new Vector3(0f, 1f, 0f);
      case KeyCode.DownArrow:  return new Vector3(0f, -1f, 0f);
      case KeyCode.RightArrow: return new Vector3(1f, 0f, 0f);
      case KeyCode.LeftArrow:  return new Vector3(-1f, 0f, 0f);
    }
    return Vector3.zero;
  }
}

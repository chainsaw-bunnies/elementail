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

  KeyCode CurrentKey;
  KeyCode NextKey;

  Vector3 CurrentDir;
  Vector3 NextDir;

  Vector3 Goal;

  void Start()
  {
    CharacterController = GetComponent<CharacterController>();
    CurrentGrounds = new HashSet<Ground>();

    CurrentKey = KeyCode.None;
    NextKey = KeyCode.None;

    CurrentDir = Vector3.zero;
    NextDir = Vector3.zero;

    Goal = transform.position;
  }

  void FixedUpdate()
  {
    // Haven't started moving yet.
    if (CurrentKey == KeyCode.None)
    {
      return;
    }

    // Move
    Speed = MaxSpeed;
    CharacterController.Move((CurrentDir * Speed * Time.fixedDeltaTime));

    // Check if you passed a goal
    var passedGoal = false;
    switch (CurrentKey)
    {
      case KeyCode.UpArrow:    passedGoal = transform.position.y >= Goal.y; break;
      case KeyCode.DownArrow:  passedGoal = transform.position.y <= Goal.y; break;
      case KeyCode.LeftArrow:  passedGoal = transform.position.x <= Goal.x; break;
      case KeyCode.RightArrow: passedGoal = transform.position.x >= Goal.x; break;
    }
    if (passedGoal)
    {
      if (CurrentKey == NextKey)
      {
        Goal += CurrentDir * Map.TileSize;
      }
      else
      {
        CurrentKey = NextKey;
        CurrentDir = NextDir;

        Goal = GetGoal(CurrentKey);
      }
    }

    // Update the direction you are facing.
    // SpriteRenderer.flipX = CurretKeyDir.x > 0;
  }

  void Update()
  {
    if (Input.GetKeyUp(KeyCode.Escape))
    {
      SceneManager.LoadScene("MainMenu");
    }

    KeyCheck(KeyCode.UpArrow,    new Vector3(0f, 1f, 0f));
    KeyCheck(KeyCode.DownArrow,  new Vector3(0f, -1f, 0f));
    KeyCheck(KeyCode.LeftArrow,  new Vector3(-1f, 0f, 0f));
    KeyCheck(KeyCode.RightArrow, new Vector3(1f, 0f, 0f));
  }
  
  void KeyCheck(KeyCode next, Vector3 nextDir)
  {
    // TODO: deal w/ reversing direction.

    if (Input.GetKeyUp(next))
    {
      if (CurrentKey == KeyCode.None)
      {
        CurrentKey = next;
        CurrentDir = nextDir;

        NextKey = next;
        NextDir = nextDir;

        Goal = GetGoal(next);
      }
      else
      {
        NextKey = next;
        NextDir = nextDir;
      }
    }
  }

  static float RoundUp(float value, float multiple)
  {
    if (!Mathf.Approximately(value % multiple, 0f))
      return (value - value % multiple) + multiple;
    return value;

  }

  static float RoundDown(float value, float multiple)
  {
    if (!Mathf.Approximately(value % multiple, 0f))
      return (value - value % multiple);
    return value;
  }

  Vector3 GetGoal(KeyCode key)
  {
    var pos = transform.position;
    switch (key)
    {
      case KeyCode.UpArrow:
        return new Vector3(pos.x, RoundUp(pos.y, Map.TileSize), 0f);

      case KeyCode.DownArrow:
        return new Vector3(pos.x, RoundDown(pos.y, Map.TileSize), 0f);

      case KeyCode.LeftArrow:
        return new Vector3(RoundDown(pos.x, Map.TileSize), pos.y, 0f);

      case KeyCode.RightAlt:
        return new Vector3(RoundUp(pos.x, Map.TileSize), pos.y, 0f);
    }
    return Vector3.zero;
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
    if (CurrentKey == KeyCode.None)
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

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
  public Animator Animator;
  public CharacterController CharacterController;
  public Image Fade;
  public Map Map;
  public GameObject PortalPrefab;
  public Renderer Renderer;

  [Header("Sounds")]
  public AudioSource AttackShort;
  public AudioSource Anxious;
  public AudioSource Damage;
  public AudioSource Happy;
  public AudioSource IdleLoop;
  public AudioSource Impact;
  public AudioSource WalkLoop;
  public AudioSource Win;

  HashSet<Ground> CurrentGrounds;
  KeyCode CurrentKey;
  KeyCode NextKey;
  Vector3 CurrentDir;
  Vector3 NextDir;
  Vector3 Goal;
  float Speed;
  bool FadingOut;
  float FadeOutTime;
  string FadeCompleteScene;
  bool Hopping;

  void Start()
  {
    CurrentGrounds = new HashSet<Ground>();
    CurrentKey = KeyCode.None;
    NextKey = KeyCode.None;
    CurrentDir = Vector3.zero;
    NextDir = Vector3.zero;
    Goal = transform.position;
    Speed = Map.PlayerStartSpeed;
    FadingOut = false;
    FadeCompleteScene = null;
  }

  void FixedUpdate()
  {
    if (FadingOut)
    {
      SilentIdling();
      return;
    }
    if (CurrentKey == KeyCode.None)
    {
      Idling();
      return;
    }

    // Move
    var prevPos = transform.position;
    CharacterController.Move((CurrentDir * Speed * Time.fixedDeltaTime));
    var stuck = Vector3.Distance(transform.position, prevPos) < (Speed * Time.fixedDeltaTime) / 10f;

    // Check if you passed a goal
    var passedGoal = false;
    switch (CurrentKey)
    {
      case KeyCode.UpArrow:    passedGoal = transform.position.y >= Goal.y; break;
      case KeyCode.DownArrow:  passedGoal = transform.position.y <= Goal.y; break;
      case KeyCode.LeftArrow:  passedGoal = transform.position.x <= Goal.x; break;
      case KeyCode.RightArrow: passedGoal = transform.position.x >= Goal.x; break;
    }
    if (passedGoal || stuck)
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

    if (stuck || CurrentDir == Vector3.zero)
    {
      Idling();
    }
    else
    {
      Running();
    }
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      SceneManager.LoadScene("MainMenu");
    }

    if (FadingOut)
    {
      // Delay the fade a little bit.
      if (Time.time > FadeOutTime + 0.5f)
      {
        var c = Fade.color;
        Fade.color = new Color(c.r, c.b, c.g, c.a + 0.002f * Time.fixedTime);
        if (Fade.color.a >= 1f)
        {
          SceneManager.LoadScene(FadeCompleteScene);
        }
      }
      return;
    }

    KeyCheck(KeyCode.UpArrow,    new Vector3(0f, 1f, 0f));
    KeyCheck(KeyCode.DownArrow,  new Vector3(0f, -1f, 0f));
    KeyCheck(KeyCode.LeftArrow,  new Vector3(-1f, 0f, 0f));
    KeyCheck(KeyCode.RightArrow, new Vector3(1f, 0f, 0f));
  }

  void KeyCheck(KeyCode next, Vector3 nextDir)
  {
    // TODO: deal w/ reversing direction.

    if (Input.GetKeyDown(next))
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
    if (CurrentKey == KeyCode.None || FadingOut || Hopping)
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
      if (ground.Dangerous)
      {
        Damage.Play();
        FadeOut("GameOver");
        return;
      }

      if (dist < bestDistance)
      {
        closest = ground;
        bestDistance = dist;
      }
    }

    if (ScoreBox.LeavesRemaining == 0)
    {
      Happy.Play();
      Win.Play();
      Instantiate(PortalPrefab, transform.position, Quaternion.identity);
      FadeOut("MainGame");
    }
    else if (closest != null && !closest.Runed)
    {
      ScoreBox.Score += 10;
      ScoreBox.LeftRunes++;
      closest.ActivateRune();
    }
  }

  #endregion

  #region Leaves

  public void Hop(Leaf leaf)
  {
    if (!leaf.Activated)
    {
      Speed += Map.PlayerSpeedIncreasePerLeaf;
    }

    Renderer.transform.localPosition = new Vector3(0f, 2f, 0f);

    AttackShort.Play();
    Hopping = true;
  }

  public void Unhop(Leaf leaf)
  {
    Impact.Play();
    Hopping = false;
    Renderer.transform.localPosition = Vector3.zero;
  }


  #endregion

  void Running()
  {
    if (IdleLoop.isPlaying)
    {
      IdleLoop.Stop();
    }
    if (!WalkLoop.isPlaying)
    {
      WalkLoop.Play();
    }

    // Update the direction the sprite is facing.
    switch (CurrentKey)
    {
		case KeyCode.UpArrow:
			Animator.SetInteger ("Direction", 0);
			Animator.Play ("BunnyUp");
        	break;
		case KeyCode.DownArrow:
			Animator.SetInteger("Direction", 1);
			Animator.Play ("BunnyDown");
			break;
		case KeyCode.LeftArrow:
			Animator.SetInteger("Direction", 2);
			Animator.Play ("BunnyLeft");
			break;
		case KeyCode.RightArrow:
			Animator.SetInteger("Direction", 3);
			Animator.Play ("BunnyRight");
			break;
		default:
			break;
    }
  }

  void Idling()
  {
    if (!IdleLoop.isPlaying)
    {
      IdleLoop.Play();
    }
    if (WalkLoop.isPlaying)
    {
      WalkLoop.Stop();
    }
    Animator.SetInteger("Direction", 4);
		Animator.Play ("Idle");
  }

  void SilentIdling()
  {
    if (IdleLoop.isPlaying)
    {
      IdleLoop.Stop();
    }
    if (WalkLoop.isPlaying)
    {
      WalkLoop.Stop();
    }
    Animator.SetInteger("Direction", 4);
	Animator.Play("Idle");

  }

  void FadeOut(string fadeCompleteScene)
  {
    FadeOutTime = Time.time;
    FadingOut = true;
    FadeCompleteScene = fadeCompleteScene;
  }
}

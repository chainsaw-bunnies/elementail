using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
  Queue<AudioSource> Used;
  Queue<AudioSource> Unused;

  void Start()
  {
    Used = new Queue<AudioSource>();
    Unused = new Queue<AudioSource>();
    for (int i = 0; i < 16; i++)
    {
      Unused.Enqueue(gameObject.AddComponent<AudioSource>());
    }
  }

  public void Play(AudioClip clip)
  {
    while (Used.Count > 0 && !Used.Peek().isPlaying)
    {
      Unused.Enqueue(Used.Dequeue());
    }

    var source = (Unused.Count > 0 ? Unused : Used).Dequeue();
    if (source.isPlaying)
    {
      source.Stop();
    }

    source.clip = clip;
    source.Play();

    Used.Enqueue(source);
  }
}


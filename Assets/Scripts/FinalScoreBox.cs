using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinalScoreBox : MonoBehaviour
{
    public Text Text;
	
	void Update ()
    {
        Text.text = "Final Score: " + ScoreBox.Score.ToString("N0");
    }
}

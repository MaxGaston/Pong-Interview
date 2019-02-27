using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Photon.PunBehaviour
{
    public int Score = 0;
    private Text Text;

    public void Increment()
    {
        Score++;
    }

    void Start()
    {
        Text = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        Text.text = Score.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : Photon.PunBehaviour
{
    public Text Left;
    public Text Right;
    public GameObject GameOverBox;
    public Text GameOverText;

    public int ScoreLimit = 3;
    
    public void Score(bool left, int ballID)
    {
        PhotonView ball = PhotonView.Find(ballID);
        if(ball.gameObject.name == "WhiteBall")
        {
            if (left)
            {
                int lscore = int.Parse(Left.text) + 1;
                Left.text = lscore.ToString();

                if (lscore >= ScoreLimit)
                {
                    GameOver(true, true);
                }
            }
            else
            {
                int rscore = int.Parse(Right.text) + 1;
                Right.text = rscore.ToString();

                if(rscore >= ScoreLimit)
                {
                    GameOver(false, true);
                }
            }
        }
        else if(ball.gameObject.name == "BlackBall")
        {
            if(left)
            {
                GameOver(true, false);
            }
            else
            {
                GameOver(false, false);
            }
        }
    }

    public void GameOver(bool leftWinner, bool scoreLimitReached)
    {
        GameOverBox.SetActive(true);

        if(leftWinner && scoreLimitReached)
        {
            GameOverText.text = "Winner: Left!";
        }
        else if(leftWinner && !scoreLimitReached)
        {
            GameOverText.text = "Winner: Left!";
        }
        else if(!leftWinner && scoreLimitReached)
        {
            GameOverText.text = "Winner: Right!";
        }
        else if(!leftWinner && !scoreLimitReached)
        {
            GameOverText.text = "Winner: Right!";
        }
    }

    public void ResetScores()
    {
        GameOverBox.SetActive(false);

        Left.text = "0";
        Right.text = "0";
    }
}

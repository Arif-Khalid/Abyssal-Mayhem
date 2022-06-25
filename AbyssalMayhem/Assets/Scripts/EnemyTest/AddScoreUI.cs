using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddScoreUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI scoreText;
    private void LateUpdate()
    {
        canvas.transform.LookAt(transform.position + Camera.main.transform.forward);
    }

    public void SetScoreAdded(int score)
    {
        scoreText.text = "+" + score.ToString();
        scoreText.fontSize *= score;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}


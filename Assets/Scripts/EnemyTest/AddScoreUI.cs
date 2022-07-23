using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddScoreUI : MonoBehaviour,IPooledObject
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] float defaultFontSize = 40f;
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
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        scoreText.text = string.Empty;
        scoreText.fontSize = defaultFontSize;
    }
}


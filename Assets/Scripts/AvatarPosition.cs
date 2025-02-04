using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarPosition : MonoBehaviour
{
    public Image avatarAI;
    public GameObject avatarAIContainer;
    private void Start()
    {
        RectTransform rectTransform = avatarAIContainer.GetComponent<RectTransform>();

        if (GameManager.instance.AI)
        {
            avatarAI.transform.rotation = Quaternion.Euler(0, 0, 0);
            avatarAIContainer.transform.rotation = Quaternion.Euler(0, 0, 0);
            rectTransform.anchoredPosition = new Vector2(-426, -415);
        }

        else
        {
            avatarAI.transform.rotation = Quaternion.Euler(0, 0, 180);
            avatarAIContainer.transform.rotation = Quaternion.Euler(0, 0, 180);
            rectTransform.anchoredPosition = new Vector2(-249.9999f, -128.4f);
        }
    }
}

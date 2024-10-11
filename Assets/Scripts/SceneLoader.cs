using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Button[] levelButtons; // Array to hold all level buttons

    void Start()
    {
        foreach (Button button in levelButtons)
        {
            button.onClick.AddListener(LoadBattlefieldScene);
        }
    }

    public void LoadBattlefieldScene()
    {
        SceneManager.LoadScene("Battlefield");
    }
}

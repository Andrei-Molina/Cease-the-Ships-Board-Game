using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private RectTransform skillPanel; // UI Panel for skills
    [SerializeField] private GameObject skillButtonPrefab; // Prefab for skill buttons
    [SerializeField] private Button deselectButton; // "X" button for deselecting
    [SerializeField] private Vector2 offset = new Vector2(100, 0); // Offset for skill UI position
    private List<Button> activeSkillButtons = new List<Button>();
    private ShipPieces selectedShip;

    public GameObject phaseCanvas;
    public GameObject deadShipCanvas;
    public GameObject logsCanvas;
    public GameObject inGameSettingsCanvas;


    private void Start()
    {
        skillPanel.gameObject.SetActive(false); // Hide skill panel initially
        deselectButton.onClick.AddListener(DeselectShip); // Add listener for the deselect button
    }

    public void ShowSkills(ShipPieces ship)
    {
        if (phaseCanvas.activeSelf || deadShipCanvas.activeSelf || logsCanvas.activeSelf || inGameSettingsCanvas.activeSelf)
        {
            Debug.Log("ShowSkills aborted: One or more canvases are active.");
            return;
        }

        ClearSkills();
        selectedShip = ship;

        int playerSkillPoints = GetPlayerSkillPoints(ship);

        foreach (var skill in ship.GetSkills())
        {
            GameObject skillButtonObj = Instantiate(skillButtonPrefab, skillPanel.transform);
            Button skillButton = skillButtonObj.GetComponent<Button>();
            TMP_Text skillButtonText = skillButtonObj.GetComponentInChildren<TMP_Text>();
            Image skillButtonImage = skillButtonObj.GetComponentInChildren<Image>();

            skillButtonText.text = $"{skill.name} (Cost: {skill.skillPointCost})";
            skillButtonImage.sprite = skill.icon;

            // Check specific skill usage indicators and add details for each ship type
            HandleSkillUsageIndicators(ship, skill, skillButtonText, skillButton);

            // Disable button if insufficient skill points
            if (playerSkillPoints < skill.skillPointCost)
            {
                skillButton.interactable = false;
            }

            skillButton.onClick.RemoveAllListeners();
            skillButton.onClick.AddListener(() =>
            {
                if (playerSkillPoints >= skill.skillPointCost)
                {
                    Debug.Log($"Executing skill: {skill.name}");
                    skill.Execute(ship);
                }
                else
                {
                    Debug.LogWarning("Insufficient skill points to execute this skill.");
                }
            });

            activeSkillButtons.Add(skillButton);
        }

        PositionSkillPanel(ship);
        skillPanel.gameObject.SetActive(true);

        GameManager.instance.EnableGameInteraction(false);
    }

    private void PositionSkillPanel(ShipPieces ship)
    {
        // Get the ship's world position
        Vector3 shipScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, ship.transform.position);

        // Convert the world position to a screen position
        RectTransform canvasRect = skillPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, shipScreenPosition, null, out Vector2 localPoint);

        // Set the skill panel's position with an offset
        skillPanel.localPosition = localPoint + offset;
    }

    public void DeselectShip()
    {
        ClearSkills();
        skillPanel.gameObject.SetActive(false);
        selectedShip = null;

        GameManager.instance.EnableGameInteraction(true);
    }

    private void ClearSkills()
    {
        foreach (Button button in activeSkillButtons)
        {
            Destroy(button.gameObject);
        }
        activeSkillButtons.Clear();
    }

    public GameObject SkillPanel()
    {
        return skillPanel.gameObject;
    }

    private void HandleSkillUsageIndicators(ShipPieces ship, Skill skill, TMP_Text skillButtonText, Button skillButton)
    {
        if (ship is Destroyer destroyer && skill.name == "Deploy Smoke")
        {
            skillButtonText.text += $" ({destroyer.remainingDeploySmokeUsage}/{destroyer.maxDeploySmokeUsage})";
            if (destroyer.remainingDeploySmokeUsage <= 0)
            {
                skillButton.interactable = false;
            }
        }
        else if (ship is DestroyerASW destroyerASW && skill.name == "Deploy Depth Charge")
        {
            skillButtonText.text += $" ({destroyerASW.remainingDeployDepthChargeUsage}/{destroyerASW.maxDeployDepthChargeUsage})";
            if (destroyerASW.remainingDeployDepthChargeUsage <= 0)
            {
                skillButton.interactable = false;
            }
        }
        else if (ship is LightCruiser lightCruiser && skill.name == "HE-Ammo Barrage")
        {
            skillButtonText.text += $" ({lightCruiser.remainingHEAmmoBarrageSkillUsage}/{lightCruiser.maxHEAmmoBarrageSkillUsage})";
            if (lightCruiser.remainingHEAmmoBarrageSkillUsage <= 0)
            {
                skillButton.interactable = false;
            }
        }
        else if (ship is Dockyard)
        {
            if (skill.name == "Repair Ship" || skill.name == "Resupply Ship")
            {
                skillButtonText.text += $" (\u221E)";
            }
        }
    }

    private int GetPlayerSkillPoints(ShipPieces ship)
    {
        return ship.IsPlayer1() ? GameManager.instance.player1SkillPoints : GameManager.instance.player2SkillPoints;
    }

    public void DeductPlayerSkillPoints(ShipPieces ship, int cost)
    {
        if (ship.IsPlayer1())
        {
            GameManager.instance.player1SkillPoints -= cost;
        }
        else
        {
            GameManager.instance.player2SkillPoints -= cost;
        }
    }
}

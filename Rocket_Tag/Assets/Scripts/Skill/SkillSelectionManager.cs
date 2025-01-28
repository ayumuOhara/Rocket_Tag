using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSelectionManager : MonoBehaviour
{
    public TMP_Text skillDescriptionText;

    public void ShowSkillDescription(string description)
    {
        skillDescriptionText.text = description;
    }
}

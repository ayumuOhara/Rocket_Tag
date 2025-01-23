using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionManager : MonoBehaviour
{
    public Text skillDescriptionText;

    public void ShowSkillDescription(string description)
    {
        skillDescriptionText.text = description;
    }
}

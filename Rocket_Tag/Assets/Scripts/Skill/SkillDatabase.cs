using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDatabase : MonoBehaviour
{
    public List<Skill> skills;

    public void DisplaySkill(int index, Text descriptionText, Image iconImage)
    {
        descriptionText.text = skills[index].description;
        iconImage.sprite = skills[index].icon;
    }
}

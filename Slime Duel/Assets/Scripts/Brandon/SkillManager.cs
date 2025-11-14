using System;
using UnityEngine;
using System.Collections.Generic;
public class SkillManager : MonoBehaviour
{
    
    // Références aux listes de skills par classe
    public List<ActionSO> guerrierSkills;
    public List<ActionSO> mageSkills;
    public List<ActionSO> assassinSkills;
    public List<ActionSO> clercSkills;
    public List<ActionSO> druideSkills;

    public void AssignSkills(SlimeUnit slime)
    {
        slime.actions.Clear(); // Réinitialise les compétences

        switch (slime.classe)
        {
            case SlimeClass.Guerrier:
                slime.actions.AddRange(guerrierSkills);
                break;
            case SlimeClass.Mage:
                slime.actions.AddRange(mageSkills);
                break;
            case SlimeClass.Assassin:
                slime.actions.AddRange(assassinSkills);
                break;
            case SlimeClass.Clerc:
                slime.actions.AddRange(clercSkills);
                break;
            case SlimeClass.Druide:
                slime.actions.AddRange(druideSkills);
                break;
        }

        Debug.Log($"{slime.slimeName} a reçu {slime.actions.Count} compétences pour la classe {slime.classe}");
    }
}

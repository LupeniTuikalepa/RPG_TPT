using UnityEngine;

public class SelectionSlimeUI : MonoBehaviour
{
    public SlimeSelection manager; // drag dans inspector

    public void SetSlime(Slime s)
    {
        manager.SetSlime(s);
    }

    public void ChoisirClasse(int classeIndex)
    {
        manager.ChoisirClasse(classeIndex);
    }
}
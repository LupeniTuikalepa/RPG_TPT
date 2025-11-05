using UnityEngine;

public enum SlimeClass
{
    Guerrier,
    Mage,
    Assassin,
    Clerc,
    Druide
}

public class Slime : MonoBehaviour
{
    [Header("Choix de la classe")]
    public SlimeClass classe;

    [Header("Bonus de stats (ajoutés aux stats de base)")]
    public int bonusPV;
    public int bonusMana;
    public int bonusAgi;
    public int bonusFor;
    public int bonusInt;
    public int bonusDef;

    // Stats finales (runtime, pas visible dans l’inspector)
    [HideInInspector] public int PV;
    [HideInInspector] public int Mana;
    [HideInInspector] public int Agi;
    [HideInInspector] public int For;
    [HideInInspector] public int Int;
    [HideInInspector] public int Def;

    void Start()
    {
        InitialiserStats();
        Debug.Log($"{gameObject.name} ({classe}) → PV:{PV} Mana:{Mana} Agi:{Agi} For:{For} Int:{Int} Def:{Def}");
    }

    void InitialiserStats()
    {
        // Stats de base selon la classe
        switch (classe)
        {
            case SlimeClass.Guerrier:
                PV = 60; Mana = 10; Agi = 8; For = 10; Int = 5; Def = 35;
                break;
            case SlimeClass.Mage:
                PV = 35; Mana = 40; Agi = 9; For = 4; Int = 16; Def = 20;
                break;
            case SlimeClass.Assassin:
                PV = 40; Mana = 15; Agi = 16; For = 17; Int = 6; Def = 20;
                break;
            case SlimeClass.Clerc:
                PV = 40; Mana = 30; Agi = 8; For = 7; Int = 15; Def = 20;
                break;
            case SlimeClass.Druide:
                PV = 45; Mana = 25; Agi = 10; For = 8; Int = 12; Def = 24;
                break;
        }

        // Ajoute les bonus du joueur
        PV += bonusPV;
        Mana += bonusMana;
        Agi += bonusAgi;
        For += bonusFor;
        Int += bonusInt;
        Def += bonusDef;
    }
}
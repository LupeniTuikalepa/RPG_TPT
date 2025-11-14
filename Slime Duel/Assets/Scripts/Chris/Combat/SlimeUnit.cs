using System;
using System.Collections;                  // <<< ajouté pour les coroutines
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SlimeUnit : MonoBehaviour
{
    [Header("Expérience et niveau")]
    public int CurrentExp = 0;
    public int NextLvl = 100;
    public int Lvl = 1;
    
    [Header("Données")]
    public string slimeName = "Slime";
    public SlimeClass classe;

    [Header("Liste des compétences connues")]
    public List<ActionSO> actions = new();

    [Header("Stats de base")]
    public Stats baseStats;

    [Header("Objet équipé (facultatif)")]
    public BaseItemSO equippedItem;
    public ItemRuntime itemRuntime;

    [Header("Runtime")]
    public int PV, PVMax, Mana, ManaMax, Agi, For, Int, Def;
    public bool IsAlive => PV > 0;

    // Mort
    public event Action<SlimeUnit> Died;
    public bool deathHandled = false;

    // Statuts actifs
    public readonly List<StatusInstance> statuses = new();

    // ======== VISUEL (flash dégâts / soin) ========
    [Header("Visuel")]
    public SpriteRenderer spriteRenderer;            // sprite du slime
    public Color hitFlashColor  = Color.red;        // couleur quand il prend des dégâts
    public Color healFlashColor = Color.green;      // couleur quand il est soigné
    public float flashDuration  = 0.12f;

    private Color baseColor = Color.white;
    private Coroutine flashRoutine;

    void Awake()
    {
        if (baseStats.PV == 0 && baseStats.Mana == 0)
        {
            switch (classe)
            {
                case SlimeClass.Guerrier: baseStats = new Stats{PV=60,Mana=10,Agi=8, For=10,Int=5, Def=35}; break;
                case SlimeClass.Mage:     baseStats = new Stats{PV=35,Mana=40,Agi=9, For=4, Int=16,Def=20}; break;
                case SlimeClass.Assassin: baseStats = new Stats{PV=40,Mana=15,Agi=16,For=17,Int=6, Def=20}; break;
                case SlimeClass.Clerc:    baseStats = new Stats{PV=60,Mana=30,Agi=8, For=7, Int=15,Def=25}; break;
                case SlimeClass.Druide:   baseStats = new Stats{PV=45,Mana=25,Agi=10,For=8, Int=12,Def=24}; break;
            }
        }

        PVMax   = PV   = Mathf.Max(1, baseStats.PV);
        ManaMax = Mana = Mathf.Max(0, baseStats.Mana);
        Agi = baseStats.Agi; For = baseStats.For; Int = baseStats.Int; Def = baseStats.Def;

        if (equippedItem != null)
        {
            equippedItem.ApplyOnEquip(this);
            itemRuntime = new ItemRuntime(equippedItem);
            equippedItem.OnEquip(this, itemRuntime);
        }

        // Visuel : récupère le SpriteRenderer si non assigné
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer)
            baseColor = spriteRenderer.color;

        ClampRuntime();
    }

    // ======== Combat Utilities ========
    public void SpendMana(int c)
    {
        Mana = Mathf.Clamp(Mana - Mathf.Max(0, c), 0, ManaMax);
    }

    public void Heal(int v)
    {
        int before = PV;
        PV = Mathf.Min(PVMax, PV + Mathf.Abs(v));

        if (PV > before)
            FlashHeal();
    }

    public void HealPercent(float p)
    {
        Heal(Mathf.CeilToInt(PVMax * Mathf.Clamp01(p)));
    }

    public int TakeDamage(int raw, DamageKind kind)
    {
        
        int dmg = kind == DamageKind.True 
            ? Mathf.Max(0, raw) 
            : Mathf.Max(1, Mathf.RoundToInt(raw * (100f / (100f + Def))));
        PV = Mathf.Max(0, PV - dmg);

        if (dmg > 0)
            FlashHit();

        equippedItem?.OnReceiveHit(this, null, dmg, itemRuntime);

        if (PV == 0 && !deathHandled) Die();

        return dmg;
    }

    // ======== Mort ========
    public void Die()
    {
        if (deathHandled) return;
        deathHandled = true;

        Debug.Log($"{slimeName} est K.O.");
        CombatLogUI.I?.Log(this, $"{slimeName} est K.O.", true);
        try { Died?.Invoke(this); } catch { /* ignore */ }
        BattleSystem.I?.OnUnitDied(this);

        gameObject.SetActive(false);
    }

    // ======== Item Hooks ========
    public void BeginBattle()        { equippedItem?.OnBattleStart(this, itemRuntime); ClampRuntime(); }
    public void BeginTurn()          { equippedItem?.OnTurnStart(this, itemRuntime);   ClampRuntime(); }
    public void OnAttack(SlimeUnit t)=> equippedItem?.OnAttack(this, t, itemRuntime);
    public void OnKill(SlimeUnit v)  { equippedItem?.OnKill(this, v, itemRuntime);     ClampRuntime(); }
    public void OnSpellCast(SlimeUnit t)=> equippedItem?.OnSpellCast(this, t, itemRuntime);

    // ======== Status System ========
    public void AddStatus(StatusSO so, int stacks = 1, int turns = 1)
    {
        statuses.Add(new StatusInstance(so, stacks, turns));
    }

    public bool HasTag(StatusTag tag)
    {
        foreach(var s in statuses)
            if (s.IsActive && s.def.HasTag(tag)) return true;
        return false;
    }

    public void TickStartOfTurn()
    {
        foreach (var s in statuses)
            if (s.IsActive) s.def.OnTurnStart(this, s);

        equippedItem?.OnTurnStart(this, itemRuntime);
    }

    public void TickEndOfTurn()
    {
        foreach (var s in statuses)
            if (s.IsActive) s.def.OnTurnEnd(this, s);

        for(int i = statuses.Count - 1; i >= 0; i--)
        {
            statuses[i].turns--;
            if (statuses[i].turns <= 0) statuses.RemoveAt(i);
        }
    }

    // ======== UTILITAIRES POUR LES OBJETS ========
    public void AddAllStats(int d)
    {
        PVMax = Mathf.Max(1, PVMax + d);
        PV    = Mathf.Clamp(PV + d, 0, PVMax);

        ManaMax = Mathf.Max(0, ManaMax + d);
        Mana    = Mathf.Clamp(Mana + d, 0, ManaMax);

        Agi  += d; For += d; Int += d; Def += d;
        ClampRuntime();
    }

    public void ClampRuntime()
    {
        PVMax   = Mathf.Max(1, PVMax);
        ManaMax = Mathf.Max(0, ManaMax);

        PV   = Mathf.Clamp(PV,   0, PVMax);
        Mana = Mathf.Clamp(Mana, 0, ManaMax);

        Agi = Mathf.Max(0, Agi);
        For = Mathf.Max(0, For);
        Int = Mathf.Max(0, Int);
        Def = Mathf.Max(0, Def);
    }
    
    public void RestoreManaPercent(float p)
    {
        int add = Mathf.CeilToInt(ManaMax * Mathf.Clamp01(p));
        Mana = Mathf.Min(ManaMax, Mana + add);
        Debug.Log($"{slimeName} régénère {add} mana ({Mana}/{ManaMax})");
    }
    
    // Monter de niveau

    public void LvlUp()
    {
        Lvl++;
        switch (classe)
        {
            case SlimeClass.Guerrier : 
                PVMax += 10;
                PV = PVMax;
                
                ManaMax += 3;
                Mana = ManaMax;

                For += 3;
                Def += 5;
                Int += 1;
                Agi += 1;
                break;
            
            case SlimeClass.Assassin :
                PVMax += 3;
                PV = PVMax;
                
                ManaMax += 3;
                Mana = ManaMax;
                
                For += 5;
                Def += 2;
                Int += 1;
                Agi += 5;
                break;
            
            case SlimeClass.Mage :
                PVMax += 3;
                PV = PVMax;
                
                ManaMax += 10;
                Mana = ManaMax;
                
                For += 2;
                Def += 2;
                Int += 5;
                Agi += 2;
                break;
            
            case SlimeClass.Clerc :
                PVMax += 5;
                PV = PVMax;
                
                ManaMax += 10;
                Mana = ManaMax;
                
                For += 3;
                Def += 2;
                Int += 5;
                Agi += 4;
                break;
            
            case SlimeClass.Druide :
                PVMax += 6;
                PV = PVMax;
                
                ManaMax += 8;
                Mana = ManaMax;
                
                For += 3;
                Def += 3;
                Int += 3;
                Agi += 3;
                break;
        }
        Debug.Log($"{slimeName} monte au niveau {Lvl} ! Stats améliorées : PV={PVMax}, Mana={ManaMax}, For={For}, Int={Int}, Agi={Agi}, Def={Def}");
        
        SkillManager manager = FindObjectOfType<SkillManager>();
        if (manager != null)
        {
            manager.AssignSkills(this);
        }
    }

    // ======== FLASH VISUEL ========
    void StartFlash(Color c)
    {
        if (!spriteRenderer) return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(c));
    }

    IEnumerator FlashRoutine(Color c)
    {
        spriteRenderer.color = c;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = baseColor;
        flashRoutine = null;
    }

    void FlashHit()  => StartFlash(hitFlashColor);
    void FlashHeal() => StartFlash(healFlashColor);
}

// Runtime status instance
public class StatusInstance
{
    public StatusSO def;
    public int stacks;
    public int turns;
    public bool IsActive => turns > 0;

    public StatusInstance(StatusSO def,int stacks,int turns)
    {
        this.def = def;
        this.stacks = stacks;
        this.turns = turns;
    }
}









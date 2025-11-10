using System;
using UnityEngine;
using System.Collections.Generic;

public class SkinLoadData : MonoBehaviour
{
    [SerializeField] List<Skin> AllSkins;
    [SerializeField] public Inventaire inventaire;

    private void Start()
    {
        for (int i = 0; i <AllSkins.Count; i++)
        {
            if(AllSkins[i]!=null) AllSkins[i].ID = i;
        }
    }

    public void Load()
    {
        SavedData LoadedData = SaveSystem.LoadData();
        if (LoadedData == null)
        {
            inventaire.Skin_Debloque = new List<Skin>();
            inventaire.Gacha_Machine.Commun_Skins = new List<Skin>();
            inventaire.Gacha_Machine.Rare_Skins = new List<Skin>();
            inventaire.Gacha_Machine.Epic_Skins = new List<Skin>();
            inventaire.Gacha_Machine.Legendary_Skins = new List<Skin>();

            foreach (var m in AllSkins)
            {
                switch (m.rarity)
                {
                    case 0:
                        inventaire.Gacha_Machine.Commun_Skins.Add(m);
                        Debug.Log("un skin a été rajouté a commun");
                        break;
                    case 1:
                        inventaire.Gacha_Machine.Rare_Skins.Add(m);
                        Debug.Log("un skin a été rajouté a rare");
                        break;
                    case 2:
                        inventaire.Gacha_Machine.Epic_Skins.Add(m);
                        Debug.Log("un skin a été rajouté a epic");
                        break;
                    case 3:
                        inventaire.Gacha_Machine.Legendary_Skins.Add(m);
                        Debug.Log("un skin a été rajouté a legendaire");
                        break;
                    default:
                        break;
                }
            }
            SaveSystem.SaveAllData(inventaire.Gacha_Machine, inventaire);
            return;
        }
        inventaire.Skin_Debloque = new List<Skin>();
        inventaire.Gacha_Machine.Commun_Skins = new List<Skin>();
        inventaire.Gacha_Machine.Rare_Skins = new List<Skin>();
        inventaire.Gacha_Machine.Epic_Skins = new List<Skin>();
        inventaire.Gacha_Machine.Legendary_Skins = new List<Skin>();
        
        foreach (var i in LoadedData.IDs_Skins_Unlocked)
        {
            foreach (var j in AllSkins)
            {
                if (j.ID == i)
                {
                    inventaire.Skin_Debloque.Add(j);
                }
            }
        } // load unlocked skins

        foreach (var k in LoadedData.IDs_Skins_locked)
        {
            foreach (var l in AllSkins)
            {
                if (l.ID == k)
                {
                    switch (l.rarity)
                    {
                        case 0:
                            inventaire.Gacha_Machine.Commun_Skins.Add(l);
                            break;
                        case 1:
                            inventaire.Gacha_Machine.Rare_Skins.Add(l);
                            break;
                        case 2:
                            inventaire.Gacha_Machine.Epic_Skins.Add(l);
                            break;
                        case 3:
                            inventaire.Gacha_Machine.Legendary_Skins.Add(l);
                            break;
                        default:
                            break;
                    }
                }
            }
        } // load locked skins
        return;
    }

    public void DeleteData()
    {
        SaveSystem.DeleteSavedData();
        Load();
    }
}
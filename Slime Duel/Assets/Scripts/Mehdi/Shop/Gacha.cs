using System.Collections.Generic;
using UnityEngine;


public class Gacha : MonoBehaviour
{
    [SerializeField] public List<Skin> Commun_Skins = new List<Skin>();
    [SerializeField] public List<Skin> Rare_Skins = new List<Skin>();
    [SerializeField] public List<Skin> Epic_Skins = new List<Skin>();
    [SerializeField] public List<Skin> Legendary_Skins = new List<Skin>();
    private int rarity_preOp = 0;
    private int rarity_pulled = 0;
    void Start()
    {
        
    }

    public Skin pull()
    {
        Skin pulled_skin = null;
        // on prend la liste des slimes disponible, on les multiplie par le ratio de rareté, on rammenne chaque rang en pourcentage

        int rarity_commun = 40 * Commun_Skins.Count;        //40 - 30 - 20 - 10
        int rarity_rare = 30 * Rare_Skins.Count;
        int rarity_epic = 20 * Epic_Skins.Count;
        int rarity_legendary = 10 * Legendary_Skins.Count;
        rarity_preOp = rarity_commun + rarity_rare + rarity_epic + rarity_legendary;
        rarity_pulled = Random.Range(1, rarity_preOp + 1);
        //if else <- pour chaque rareté      commun, commun + rare, commun + rare + epic, ...
        return pulled_skin;
    }
    
    void Update()
    {
        
    }
}

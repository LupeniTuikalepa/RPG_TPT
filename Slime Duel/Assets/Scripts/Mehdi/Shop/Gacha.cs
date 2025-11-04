using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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

    public void pull()
    {
        //rarity_preOp = 
        //rarity_pulled = 
    }
    
    void Update()
    {
        
    }
}

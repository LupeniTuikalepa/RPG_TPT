using UnityEngine;
using System.Collections.Generic;

public class Inventaire : MonoBehaviour
{
    [SerializeField] public Gacha Gacha_Machine;
    public List<Skin> Skin_Debloque = new List<Skin>();


    public void try_pull()
    {
        Skin unlocked_skin = Gacha_Machine.pull();
        Skin_Debloque.Add(unlocked_skin);
        Debug.Log("new skin: " + unlocked_skin.name);
        Debug.Log("current skins unlocked: " + Skin_Debloque.Count);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterInfo{
    public int experience = 0;
    public int level = 1;
    public int position = -1; //This is entirely for Players, 0 = Leader
    public int CurrentHealth = 0;
    public int MaxHealth = 0;
    public int CurrentMagicPoints = 0;
    public int MaxMagicPoints = 0;
    public int PhysAttack = 0;
    public int PhysDefense = 0;
    public int MagAttack = 0;
    public int MagDefense = 0;
    public int Speed = 0;
    public CharaType Chara = CharaType.PLAYER;
    public List<string> Skills = new List<string>();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


public enum CharaType { ENEMY, PLAYER }

public class Entity : MonoBehaviour {
    
    public string EntityName;
    public int experience = 0;
    public int level = 1;
    public int position = -1; //This is entirely for Players, 0 = Leader
    public int CurrentHealth;
    public int MaxHealth;
    public int CurrentMagicPoints;
    public int MaxMagicPoints;
    public int PhysAttack;
    public int PhysDefense;
    public int MagAttack;
    public int MagDefense;
    public int Speed;
    public CharaType Chara;
    public List<Element> Element;
    public List<Skill> Skills;
    
    /*private ShakeEffect shake;

    void Awake()
    {
        shake = GetComponent<ShakeEffect>();
    }*/

    public void OnTurn()
    {
        /*foreach (Effect status in Effects)
        {
            if(status.Turns != 0)
            {
                status.Turns -= 1;
                
            }
            status.ability();
        }*/
    }

    public bool CastSpell(Skill s, Entity e)
    {
        bool cast = CurrentMagicPoints >= s.Cost;

        if (cast)
        {
            //Uncomment this once I add particle effects. for now just need the spell to deal damage
            //Skill toCast = Instantiate<Skill>(s, transform.position, Quaternion.identity);
            CurrentMagicPoints -= s.Cost;
            s.Cast(this, e);
        }

        return cast;
    }

    public bool CastAOE(Skill s, List<Entity> e)
    {
        bool cast = CurrentMagicPoints >= s.Cost;

        if (cast)
        {
            //Uncomment this once I add particle effects. for now just need the spell to deal damage
            //Skill toCast = Instantiate<Skill>(s, transform.position, Quaternion.identity);
            CurrentMagicPoints -= s.Cost;
            s.CastAOE(this, e);
        }

        return cast;
    }

    public void Damage(int amt)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - amt, 0);
        //shake.Shake(this.GetComponent<RectTransform>(), 1.0f, 2.0f);
        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amt)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amt, MaxHealth);
    }

    public bool alive
    {
        get
        {
            return CurrentHealth > 0;
        }
    }

    public virtual void Die()
    {
        Debug.LogFormat("{0} has died.", EntityName);
        for(int i = 0; i<BattleWindow.Instance.ActiveBattleMembers.Count; i++)
        {
            if (BattleWindow.Instance.ActiveBattleMembers[i].Equals(this))
            {
                if(BattleWindow.Instance.currentTurn > i)
                {
                    BattleWindow.Instance.currentTurn--;
                    break;
                }
            }
        }
    }

    public virtual void CheckLevelUp()
    {
        //Solely for PartyMembers
    }

    public virtual void LevelUp()
    {
        //Solely for PartyMembers
    }

    public virtual void Respec()
    {
        //Solely for Enemy
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Directory.CreateDirectory(Application.persistentDataPath + "/PlayerInfo/");
        FileStream fs = File.Open(Application.persistentDataPath + "/PlayerInfo/" + this.EntityName + ".dat", FileMode.Create);

        CharacterInfo ci = new CharacterInfo();
        ci.experience = this.experience;
        ci.level = this.level;
        ci.position = this.position;
        ci.CurrentHealth = this.CurrentHealth;
        //Debug.Log(ci.CurrentHealth);
        ci.MaxHealth = this.MaxHealth;
        ci.CurrentMagicPoints = this.CurrentMagicPoints;
        ci.MaxMagicPoints = this.MaxMagicPoints;
        ci.PhysAttack = this.PhysAttack;
        ci.PhysDefense = this.PhysDefense;
        ci.MagAttack = this.MagAttack;
        ci.MagDefense = this.MagDefense;
        ci.Speed = this.Speed;
        ci.Chara = this.Chara;
        //ci.Effects = Effects;

        bf.Serialize(fs, ci);
        fs.Close();
    }

    public void LoadData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + "/PlayerInfo/" + this.EntityName + ".dat", FileMode.Open);
        CharacterInfo ci = (CharacterInfo)bf.Deserialize(fs);
        fs.Close();

        this.experience = ci.experience;
        this.level = ci.level;
        this.position = ci.position;
        this.CurrentHealth = ci.CurrentHealth;
        //Debug.Log(this.CurrentHealth);
        this.MaxHealth = ci.MaxHealth;
        this.CurrentMagicPoints = ci.CurrentMagicPoints;
        this.MaxMagicPoints = ci.MaxMagicPoints;
        this.PhysAttack = ci.PhysAttack;
        this.PhysDefense = ci.PhysDefense;
        this.MagAttack = ci.MagAttack;
        this.MagDefense = ci.MagDefense;
        this.Speed = ci.Speed;
        this.Chara = ci.Chara;
        //Effects = ci.Effects;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleWindow : GenericWindow {

    public static BattleWindow Instance { get; set; }

    [System.Serializable]
    public struct BattleSpawnGroups
    {
        [SerializeField] public BattleSpawnPoint[] SpawnPoints;
    }

    [SerializeField]
    private List<BattleSpawnGroups> SpawnGroups;
    //[SerializeField]
    //private GameObject spellPanel;
    [SerializeField]
    private Button[] actionButtons;
    /*[SerializeField]
    private Button button;*/
    [SerializeField]
    public Text[] entityInfo;
    [SerializeField]
    private Text action;

    public List<Entity> ActiveBattleMembers = new List<Entity>();
    public int currentTurn = 0;
    public Skill playerSelected;
    public bool playerAttacking;

    //Use for field menu
    public bool InBattle = false;

    public delegate void BattleOver(bool playerWin);
    public BattleOver battleOverCall;

    private SpellWindow spellWindow;
    private TargetWindow targetWindow;

    //private ShakeEffect shakeEffect;

    //Used only if the player wins the battle
    private int EXP = 0;

    /*protected override void Awake()
    {
        shakeEffect = GetComponent<ShakeEffect>();
        base.Awake();
    }*/

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        //spellPanel.SetActive(false);
    }

    public void StartBattleBoss(List<Entity> c, List<Entity> e, int section)
    {
        foreach (Entity pm in c)
        {
            if (!System.IO.File.Exists(Application.persistentDataPath + "/PlayerInfo/" + pm.EntityName + ".dat"))
            {
                pm.SaveData();
                //Debug.Log(string.Format("{0} data file has been created", pm.EntityName));
            }
            //Debug.Log(Application.persistentDataPath);
            pm.LoadData();
        }
        int activeParty = c.Count > 4 ? 4 : c.Count;

        c.Sort((x1, x2) => x1.position.CompareTo(x2.position));

        for (int i = 0; i < activeParty; i++)
        {
            ActiveBattleMembers.Add(SpawnGroups[0].SpawnPoints[i].spawn(c[i]));
            //ActiveBattleMembers.Add(SpawnPoints[i + 4].spawn(c[i]));
        }

        Entity spawn = getEntityToSpawn(e);
        ActiveBattleMembers.Add(SpawnGroups[1].SpawnPoints[1].spawn(spawn));

        currentTurn = 0;
        ActiveBattleMembers.Sort((x1, x2) => x1.Speed.CompareTo(x2.Speed));
        ActiveBattleMembers.Reverse();
        UpdateAction(string.Format("Battle has started against the Boss!"));
        ToggleActionState(false);
        UpdateCharUI();
        //spellPanel.SetActive(false);
        Invoke("NextAct", 3);
    }

    public void StartBattle(List<Entity> c, List<MapMaker.EnemySpawns> e, int floor)
    {
        InBattle = true;
        foreach (Entity pm in c)
        {
            if (!System.IO.File.Exists(Application.persistentDataPath + "/PlayerInfo/" + pm.EntityName + ".dat"))
            {
                pm.SaveData();
                //Debug.Log(string.Format("{0} data file has been created", pm.EntityName));
            }
            //Debug.Log(Application.persistentDataPath);
            pm.LoadData();
        }
        int activeParty = c.Count > 4 ? 4 : c.Count;

        c.Sort((x1, x2) => x1.position.CompareTo(x2.position));

        for (int i = 0; i < activeParty; i++)
        {
            ActiveBattleMembers.Add(SpawnGroups[0].SpawnPoints[i].spawn(c[i]));
            //ActiveBattleMembers.Add(SpawnPoints[i + 4].spawn(c[i]));
        }

        int inBattle = Random.Range(1, 3);
        for (int i = 0; i < inBattle; i++)
        {
            Entity spawn = getEntityToSpawn(e[floor - 1].enemies);
            ActiveBattleMembers.Add(SpawnGroups[inBattle].SpawnPoints[i].spawn(spawn));
            //ActiveBattleMembers.Add(SpawnPoints[i].spawn(spawn));
        }

        currentTurn = 0;
        ActiveBattleMembers.Sort((x1, x2) => x1.Speed.CompareTo(x2.Speed));
        ActiveBattleMembers.Reverse();
        UpdateAction(string.Format("Battle has started!"));
        ToggleActionState(false);
        UpdateCharUI();
        //spellPanel.SetActive(false);
        Invoke("NextAct", 3);
    }

    private Entity getEntityToSpawn(List<Entity> enemies)
    {
        return enemies[Random.Range(0, enemies.Count)];
    }

    private void NextTurn()
    {
        if (currentTurn + 1 > ActiveBattleMembers.Count - 1)
        {
            currentTurn = 0;
        }
        else
        {
            currentTurn += 1;
        }
    }

    private void NextAct()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER) && x.alive);
        List<Entity> e = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.ENEMY));
        if (c.Count == 0 || e.Count == 0)
        {
            //NextTurn();
            //Debug.Log(string.Format("Current Turn: {0} with {1} as the turn entity", currentTurn, GetCurrentCharacter().EntityName));
            //Destroy(Instance);
            //Debug.Log("Battle Has Ended");
            if (c.Count == 0)
            {
                for (int i = 0; i < ActiveBattleMembers.Count; i++)
                {
                    Destroy(ActiveBattleMembers[i].gameObject);
                }
                ActiveBattleMembers.Clear();
                UpdateAction("And they were never heard from again...");
                battleOverCall(false);
            }
            else if (e.Count == 0)
            {
                StartCoroutine(PlayerWin());
            }
        }
        else
        {
            if (GetCurrentCharacter().Chara.Equals(CharaType.PLAYER))
            {
                SelectButton(false);
                if (GetCurrentCharacter().alive)
                {
                    ToggleActionState(true);
                    //BuildSpellList(GetCurrentCharacter().Skills);
                    UpdateAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName));
                }
                else
                {
                    NextTurn();
                    NextAct();
                }
            }
            else
            {
                //Debug.Log("Should be an enemy turn: " + currentTurn);
                ToggleActionState(false);
                StartCoroutine(PerformAct());
            }
        }        
    }

    // Player Controllers

    public void SelectCharacter(Entity e)
    {
        if (playerAttacking)
        {
            DoAttack(GetCurrentCharacter(), e);
            targetWindow.Close();
            ToggleActionState(false);
            playerAttacking = false;
            if(GetCurrentCharacter().CurrentMagicPoints != GetCurrentCharacter().MaxMagicPoints){
                Debug.Log(string.Format("Character: {0} | Current Magic Points {1}", GetCurrentCharacter().EntityName, GetCurrentCharacter().CurrentMagicPoints));
                if(GetCurrentCharacter().CurrentMagicPoints + 3 < GetCurrentCharacter().MaxMagicPoints)
                {
                    GetCurrentCharacter().CurrentMagicPoints += 3;
                }
                else
                {
                    GetCurrentCharacter().CurrentMagicPoints = GetCurrentCharacter().MaxMagicPoints;
                }
                Debug.Log(string.Format("Character: {0} | Current Magic Points {1}", GetCurrentCharacter().EntityName, GetCurrentCharacter().CurrentMagicPoints));

            }
            UpdateCharUI();
            NextTurn();
            Invoke("NextAct", 3);
            //NextAct();
        }
        else if (playerSelected != null)
        {
            if (GetCurrentCharacter().CastSpell(playerSelected, e))
            {
                targetWindow.Close();
                UpdateCharUI();
                ToggleActionState(false);
                Invoke("NextAct", 3);
                NextTurn();
                playerSelected = null;
                //NextAct();
            }
            else
            {
                UpdateAction("Not Enough Mana to Cast " + playerSelected.SpellName);
                Debug.LogWarning("ERROR: Not Enough Mana to Cast " + playerSelected.SpellName);
                playerSelected = null;
                StartCoroutine(ResetAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName)));
            }
        }
        new WaitForSeconds(2);
    }

    public void DoAttack(Entity caster, Entity target)
    {
        caster.CastSpell(caster.Skills[0], target);
        //shakeEffect.Shake(target.GetComponent<RectTransform>());
    }

    // Enemy Controllers

    public Entity GetRandomPlayer()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER) && x.alive);
        //Debug.Log(string.Format("This List has {0} elements", c.Count));
        return c[Random.Range(0, c.Count - 1)];
    }

    public Entity GetWeakestEnemy()
    {

        List<Entity> e = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.ENEMY));
        Entity weakest = e[0];
        foreach (Entity x in e)
        {
            if (x.CurrentHealth < weakest.CurrentHealth)
            {
                weakest = x;
            }
        }
        return weakest;
    }

    public Entity GetCurrentCharacter()
    {
        return ActiveBattleMembers[currentTurn];
    }

    IEnumerator PerformAct()
    {
        //yield return new WaitForSeconds(3.0f);
        if (ActiveBattleMembers[currentTurn].alive &&
            ActiveBattleMembers[currentTurn].Chara.Equals(CharaType.ENEMY))
        {
            GetCurrentCharacter().GetComponent<Enemy>().Act();
            //GetCurrentCharacter().Act();
        }
        UpdateCharUI();
        yield return new WaitForSeconds(2.0f);
        NextTurn();
        NextAct();
    }

    //UI UPDATES

    public void UpdateCharUI()
    {
        List<Entity> c = ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER));
        for (int i = 0; i < c.Count; i++)
        {
            Entity p = c[i];
            entityInfo[i].text = string.Format("{0} - HP: {1}/{2} | MP: {3}/{4}", p.EntityName, p.CurrentHealth, p.MaxHealth, p.CurrentMagicPoints, p.MaxMagicPoints);
        }
    }

    public void UpdateAction(string s)
    {
        action.text = s;
    }

    IEnumerator ResetAction(string s)
    {
        yield return new WaitForSeconds(1.5f);
        action.text = s;
    }

    //Open Windows
    public void OpenSkillWindow(bool fromTarget)
    {
        if (fromTarget)
        {
            targetWindow.Close();
        }
        if(GetCurrentCharacter().Skills.Count > 1)
        {
            ToggleActionState(false);
            spellWindow = GenericWindow.manager.Open((int)Windows.SpellWindow - 1, false) as SpellWindow;
            spellWindow.BuildSpellList(GetCurrentCharacter().Skills.FindAll(x => x.UnlockLevel <= GetCurrentCharacter().level && !x.SpellName.Equals("Basic Attack")));
        }
    }

    public void OpenTargetWindow()
    {
        targetWindow = GenericWindow.manager.Open((int)Windows.TargetWindow - 1, false) as TargetWindow;
        if (playerSelected == null)
        {
            targetWindow.CreateTargetList(ActiveBattleMembers.FindAll(x => x.Chara == CharaType.ENEMY));
        }
        else if (playerSelected.Type == Skill.SpellType.HEAL)
        {
            targetWindow.CreateTargetList(ActiveBattleMembers.FindAll(x => x.Chara == CharaType.PLAYER && x.alive && x.CurrentHealth != x.MaxHealth));
        }
        else if (playerSelected.Type == Skill.SpellType.REVIVE)
        {
            targetWindow.CreateTargetList(ActiveBattleMembers.FindAll(x => x.Chara == CharaType.PLAYER && !x.alive));
        }
        else if (playerSelected.Type == Skill.SpellType.ATTACK)
        {
            targetWindow.CreateTargetList(ActiveBattleMembers.FindAll(x => x.Chara == CharaType.ENEMY));
        }
    }

    public void SelectButton(bool bc)
    {
        if (bc)
        {
            firstSelected = actionButtons[1].gameObject;
        }
        else
        {
            firstSelected = actionButtons[0].gameObject;
        }
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void ToggleActionState(bool state)
    {
        foreach (Button b in actionButtons)
        {
            b.interactable = state;
        }
    }

    //End of Battle

    public void GiveEXP(int exp)
    {
        EXP += exp;
    }

    IEnumerator PlayerWin()
    {
        UpdateAction("You have won the battle!"); // Initial Statement
        yield return new WaitForSeconds(1.0f);
        if (ActiveBattleMembers[0].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[0].EntityName, EXP)); // Position 0 EXP Gain
            ActiveBattleMembers[0].experience += EXP;
            if (ActiveBattleMembers[0].CurrentMagicPoints + (int)(ActiveBattleMembers[0].MaxMagicPoints * .3) < ActiveBattleMembers[0].MaxMagicPoints)
            {
                ActiveBattleMembers[0].CurrentMagicPoints += (int)(ActiveBattleMembers[0].MaxMagicPoints*.3);
            }
            else
            {
                ActiveBattleMembers[0].CurrentMagicPoints = ActiveBattleMembers[0].MaxMagicPoints;
            }
            UpdateCharUI();
            ActiveBattleMembers[0].CheckLevelUp();
            //yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(4.0f);
        }
        ActiveBattleMembers[0].SaveData();
        if (ActiveBattleMembers[1].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[1].EntityName, EXP)); // Position 1 EXP Gain
            ActiveBattleMembers[1].experience += EXP;
            if (ActiveBattleMembers[1].CurrentMagicPoints + (int)(ActiveBattleMembers[1].MaxMagicPoints * .3) < ActiveBattleMembers[1].MaxMagicPoints)
            {
                ActiveBattleMembers[1].CurrentMagicPoints += (int)(ActiveBattleMembers[1].MaxMagicPoints * .3);
            }
            else
            {
                ActiveBattleMembers[1].CurrentMagicPoints = ActiveBattleMembers[1].MaxMagicPoints;
            }
            UpdateCharUI();
            ActiveBattleMembers[1].CheckLevelUp();
            //yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(4.0f);
        }
        ActiveBattleMembers[1].SaveData();
        if (ActiveBattleMembers[2].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[2].EntityName, EXP)); // Position 2 EXP Gain
            ActiveBattleMembers[2].experience += EXP;
            if (ActiveBattleMembers[2].CurrentMagicPoints + (int)(ActiveBattleMembers[2].MaxMagicPoints * .3) < ActiveBattleMembers[2].MaxMagicPoints)
            {
                ActiveBattleMembers[2].CurrentMagicPoints += (int)(ActiveBattleMembers[2].MaxMagicPoints * .3);
            }
            else
            {
                ActiveBattleMembers[2].CurrentMagicPoints = ActiveBattleMembers[2].MaxMagicPoints;
            }
            UpdateCharUI();
            ActiveBattleMembers[2].CheckLevelUp();
            //yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(4.0f);
        }
        ActiveBattleMembers[2].SaveData();
        if (ActiveBattleMembers[3].alive)
        {
            UpdateAction(string.Format("{0} has gained {1} EXP", ActiveBattleMembers[3].EntityName, EXP)); // Position 3 EXP Gain
            ActiveBattleMembers[3].experience += EXP;
            if (ActiveBattleMembers[3].CurrentMagicPoints + (int)(ActiveBattleMembers[3].MaxMagicPoints * .3) < ActiveBattleMembers[3].MaxMagicPoints)
            {
                ActiveBattleMembers[3].CurrentMagicPoints += (int)(ActiveBattleMembers[3].MaxMagicPoints * .3);
            }
            else
            {
                ActiveBattleMembers[3].CurrentMagicPoints = ActiveBattleMembers[3].MaxMagicPoints;
            }
            UpdateCharUI();
            ActiveBattleMembers[3].CheckLevelUp();
            //yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(4.0f);
        }
        ActiveBattleMembers[3].SaveData();
        //Item / Gold drops
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < ActiveBattleMembers.Count; i++)
        {
            Destroy(ActiveBattleMembers[i].gameObject);
        }
        EXP = 0;
        ActiveBattleMembers.Clear();
        battleOverCall(true);
        InBattle = false;

    }

    public void FleeButton()
    {
        UpdateAction("The party chickened out!");
        List<Entity> players = ActiveBattleMembers.FindAll(x => x.Chara == CharaType.PLAYER);
        foreach (Entity p in players)
        {
            p.SaveData();
        }
        Invoke("FleeBattle", 2.0f);
    }

    private void FleeBattle()
    {
        for (int i = 0; i < ActiveBattleMembers.Count; i++)
        {
            Destroy(ActiveBattleMembers[i].gameObject);
        }
        EXP = 0;
        ActiveBattleMembers.Clear();
        battleOverCall(true);
        InBattle = false;
    }

    // Boolean Controllers

    public void SelectAttack()
    {
        playerSelected = null;
        //ToggleSpellPanel(false);
        playerAttacking = true;
        ToggleActionState(false);
        OpenTargetWindow();
    }

    public void SelectSpell(Skill spell)
    {
        playerSelected = spell;
        playerAttacking = false;
        if(GetCurrentCharacter().CurrentMagicPoints < spell.Cost)
        {
            UpdateAction("Not Enough Mana to Cast " + playerSelected.SpellName);
            Debug.LogWarning("ERROR: Not Enough Mana to Cast " + playerSelected.SpellName);
            playerSelected = null;
            StartCoroutine(ResetAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName)));
        }
        else
        {
            spellWindow.Close();
            if (spell.Target == Skill.TargetType.SINGLE)
            {
                OpenTargetWindow();
            }
            else if (spell.Target == Skill.TargetType.AOE)
            {
                List<Entity> temp = new List<Entity>();
                if (spell.Type == Skill.SpellType.ATTACK)
                {
                    temp = ActiveBattleMembers.FindAll(x => x.Chara == CharaType.ENEMY);
                }
                else if (spell.Type == Skill.SpellType.HEAL)
                {
                    temp = ActiveBattleMembers.FindAll(x => x.Chara == CharaType.PLAYER && x.alive);
                }

                if (GetCurrentCharacter().CastAOE(playerSelected, temp))
                {
                    UpdateCharUI();
                    ToggleActionState(false);
                    Invoke("NextAct", 3);
                    NextTurn();
                    playerSelected = null;
                    //NextAct();
                }
                else
                {
                    UpdateAction("Not Enough Mana to Cast " + playerSelected.SpellName);
                    //Debug.LogWarning("ERROR: Not Enough Mana to Cast " + playerSelected.SpellName);
                    playerSelected = null;
                    StartCoroutine(ResetAction(string.Format("What Will {0} Do?", GetCurrentCharacter().EntityName)));
                }
            }
        }
    }
}

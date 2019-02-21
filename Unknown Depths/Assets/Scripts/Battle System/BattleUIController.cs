using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour {

    public static BattleUIController BATTLE_UI_CONTROLLER { get; set; }

    [SerializeField]
    private GameObject spellPanel;
    [SerializeField]
    private Button[] actionButtons;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text[] entityInfo;
    [SerializeField]
    private Text action;

    private void Start()
    {
        if (BATTLE_UI_CONTROLLER != null && BATTLE_UI_CONTROLLER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            BATTLE_UI_CONTROLLER = this;
        }
        spellPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //This may be useless, will change later
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(r.origin, r.direction);
            if (hitInfo.collider != null && hitInfo.collider.CompareTag("Entity"))
            {
                Debug.Log("You Clicked an Entity");
                BattleController.BATTLE_CONTROLLER.SelectCharacter(hitInfo.collider.GetComponent<Entity>());
            }
        }
    }

    public void UpdateCharUI()
    {
        List<Entity> c = BattleController.BATTLE_CONTROLLER.ActiveBattleMembers.FindAll(x => x.Chara.Equals(CharaType.PLAYER));
        for(int i = 0; i<c.Count; i++)
        {
            Entity p = c[i];
            entityInfo[i].text = string.Format("{0} - HP: {1}/{2} | MP: {3}/{4}", p.EntityName, p.CurrentHealth, p.MaxHealth, p.CurrentMagicPoints, p.MaxMagicPoints);
        }
    }

    public void UpdateAction(string s)
    {
        action.text = s;
    }

    public void ToggleSpellPanel(bool state)
    {
        spellPanel.SetActive(state);
        if(state == true)
        {
            BuildSpellList(BattleController.BATTLE_CONTROLLER.GetCurrentCharacter().Skills);
        }
    }

    public void ToggleActionState(bool state)
    {
        ToggleSpellPanel(state);
        foreach(Button b in actionButtons)
        {
            b.interactable = state;
        }
    }

    public void BuildSpellList(List<Skill> spells)
    {
        //Remove visable spells in spell panel
        if(spellPanel.transform.childCount > 0)
        {
            foreach(Button b in spellPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }

        foreach(Skill s in spells)
        {
            if(!s.SpellName.Equals("Basic Attack"))
            {
                Button spellButton = Instantiate<Button>(button, spellPanel.transform);
                spellButton.GetComponentInChildren<Text>().text = s.SpellName;
                spellButton.onClick.AddListener(() => SelectSpell(s));
            }
        }
    }

    public void SelectAttack()
    {
        BattleController.BATTLE_CONTROLLER.playerSelected = null;
        BattleController.BATTLE_CONTROLLER.playerAttacking = true;
    }

    private void SelectSpell(Skill spell)
    {
        BattleController.BATTLE_CONTROLLER.playerSelected = spell;
        BattleController.BATTLE_CONTROLLER.playerAttacking = false;
    }
}

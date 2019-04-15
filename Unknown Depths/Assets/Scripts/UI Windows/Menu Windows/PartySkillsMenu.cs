using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartySkillsMenu : GenericWindow
{
    [SerializeField]
    private List<Button> partyButtons;
    [SerializeField]
    private GameObject spellPanel;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text spellName;
    [SerializeField]
    private Text spellText;

    private bool inSpellPanel = false;
    private bool inTargetMenu = false;
    private Entity entity = null;
    [SerializeField]
    private PartySkillsTarget pst;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && !inSpellPanel)
        {
            manager.Open((int)Windows.MenuWindow - 1, false);
            Close();
        }
        else if(Input.GetButtonDown("Cancel") && inSpellPanel && !pst.inFocus)
        {
            ClearSpells();
            inSpellPanel = false;
        }
    }

    private void ClearSpells()
    {
        if (spellPanel.transform.childCount > 0)
        {
            foreach (Button b in spellPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }
        spellName.text = "";
        spellText.text = "";
        //Debug.Log(entity.EntityName);
        Button temp = partyButtons.Find(x => x.GetComponentInChildren<Text>().text.Equals(entity.EntityName));
        //Debug.Log(temp.GetComponentInChildren<Text>());
        firstSelected = temp.gameObject;
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void DisplaySkills(string selected)
    {
        if (spellPanel.transform.childCount > 0)
        {
            foreach (Button b in spellPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }

        entity = MapMaker.Instance.Players.Find(x => x.EntityName.Equals(selected));

        List<Skill> temp = entity.Skills.FindAll(x => x.UnlockLevel <= entity.level && !x.SpellName.Equals("Basic Attack"));

        foreach(Skill s in temp)
        {
            Button spellButton = Instantiate<Button>(button, spellPanel.transform);
            spellButton.GetComponentInChildren<Text>().text = s.SpellName;
            if(s.Type == Skill.SpellType.HEAL || s.Type == Skill.SpellType.REVIVE)
            {
                spellButton.onClick.AddListener(() => OpenCastPanel(entity, s));
            }
            EventTrigger trigger = spellButton.gameObject.AddComponent<EventTrigger>();
            var hover = new EventTrigger.Entry();
            var select = new EventTrigger.Entry();
            hover.eventID = EventTriggerType.PointerEnter;
            select.eventID = EventTriggerType.Select;
            hover.callback.AddListener((e) => DisplayInformation(s));
            select.callback.AddListener((e) => DisplayInformation(s));
            trigger.triggers.Add(hover);
            trigger.triggers.Add(select);

            var nav = spellButton.navigation;
            nav.mode = Navigation.Mode.Automatic;
            spellButton.navigation = nav;
        }

        Button[] btemp = spellPanel.transform.GetComponentsInChildren<Button>();
        firstSelected = btemp[0].gameObject;
        eventSystem.SetSelectedGameObject(firstSelected);
        inSpellPanel = true;
    }
        
    private void DisplayInformation(Skill s)
    {
        spellName.text = string.Format("{0}", s.SpellName);
        spellText.text = string.Format("Cost: {0}\n{1}", s.Cost, s.Description);
    }

    private void OpenCastPanel(Entity e, Skill s)
    {
        List<Entity> temp = MapMaker.Instance.Players.FindAll(x => x.CurrentHealth < x.MaxHealth);
        //Debug.Log(temp.Count);
        /*foreach(Entity t in temp)
        {
            Debug.Log(string.Format("{0}: {1}", t.EntityName, t.CurrentHealth));
        }*/
        if(temp.Count > 0)
        {
            pst.Open();
            pst.entities = temp;
            pst.selectedSpell = s;
            pst.selectedEntity = e;
            pst.createButtons();
        }
    }
}

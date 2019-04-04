using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetWindow : GenericWindow
{
    [SerializeField]
    private GameObject targetPanel;
    [SerializeField]
    private Button button;

    void Update()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            if(BattleWindow.Instance.playerSelected != null)
            {
                BattleWindow.Instance.OpenSkillWindow(true);
            }
            else
            {
                BattleWindow.Instance.ToggleActionState(true);
                Close();
            }
        }
    }

    public void CreateTargetList(List<Entity> targets)
    {
        if (targetPanel.transform.childCount > 0)
        {
            foreach (Button b in targetPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }

        bool selected = false;
        foreach (Entity e in targets)
        {
            Button targetButton = Instantiate<Button>(button, targetPanel.transform);
            targetButton.GetComponentInChildren<Text>().text = e.EntityName;
            targetButton.onClick.AddListener(() => BattleWindow.Instance.SelectCharacter(e));
            var nav = targetButton.navigation;
            nav.mode = Navigation.Mode.Automatic;
            targetButton.navigation = nav;
        }

        Button[] temp = targetPanel.transform.GetComponentsInChildren<Button>();
        firstSelected = temp[0].gameObject;
    }

    public override void Close()
    {
        if (targetPanel.transform.childCount > 0)
        {
            foreach (Button b in targetPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }
        base.Close();
    }
}

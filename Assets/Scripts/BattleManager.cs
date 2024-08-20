using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { none, prep, fighting}

public class Combatant
{
    public CreatureSO creatureInfo;
    public bool ally;
    public bool fainted;
    public float hp;

    public int selectedMove;
    // optional stat modifiers

    public Combatant (CreatureSO _creature, bool _ally)
    {
        creatureInfo = _creature;
        fainted = false;
        hp = creatureInfo.MaxHp;
        ally = _ally;

        selectedMove = -1;
    }
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] GameObject Content;
    [SerializeField] Image Background;
    [SerializeField] TextMeshProUGUI InfoBox;
    [SerializeField] GameObject enemyUI;
    [SerializeField] GameObject userUI;

    [Space]
    [SerializeField] GameObject SelectionsMenu;
    [SerializeField] Button Fight;
    [SerializeField] Button Run;
    
    [Space]
    [SerializeField] GameObject MoveMenu;
    [SerializeField] GameObject Move1;
    [SerializeField] GameObject Move2;
    [SerializeField] GameObject Move3;
    [SerializeField] GameObject Move4;
    [SerializeField] Button BackButton;
    [SerializeField] Button AttackButton;

    // user 
    [Space]
    [SerializeField] Image userSprite;
    [SerializeField] TextMeshProUGUI userCreatureName;
    [SerializeField] Image userHealthBar;
    [SerializeField] Combatant userCombatant;

    // enemy
    [Space]
    [SerializeField] Image enemySprite;
    [SerializeField] TextMeshProUGUI enemyCreatureName;
    [SerializeField] Image enemyHealthBar;
    [SerializeField] Combatant enemyCombatant;

    // game
    private BattleState state;

    public static BattleManager Instance { get; private set; }

    public event Action OnBattleStart;
    public event Action OnBattleStop;

    [Space]
    [SerializeField] CreatureSO user;
    [SerializeField] CreatureSO enemy;


    private void Awake()
    {
        Instance = this;
        Content.SetActive(false);
        state = BattleState.none;

        Fight.onClick.AddListener(OpenMoveMenu);

        Move1.GetComponent<Button>().onClick.AddListener(SelectMove1);
        Move2.GetComponent<Button>().onClick.AddListener(SelectMove2);
        Move3.GetComponent<Button>().onClick.AddListener(SelectMove3);
        Move4.GetComponent<Button>().onClick.AddListener(SelectMove4);
        BackButton.onClick.AddListener(BackToBattlePanel);
        AttackButton.onClick.AddListener(StartBattlePhase);
    }

    private void Start()
    {
        StartCoroutine(StartBattle(user, enemy));
    }

    public IEnumerator StartBattle(CreatureSO _userC, CreatureSO _enemyC)
    {
        yield return null;
        userCombatant = new(_userC, true);
        enemyCombatant = new(_enemyC, false);

        userUI.SetActive(true);
        enemyUI.SetActive(true);

        userSprite.sprite = userCombatant.creatureInfo.Sprite;
        enemySprite.sprite = enemyCombatant.creatureInfo.Sprite;
        userCreatureName.text = userCombatant.creatureInfo.Name;
        enemyCreatureName.text = enemyCombatant.creatureInfo.Name;
        userHealthBar.fillAmount = 1;
        enemyHealthBar.fillAmount = 1;
        userCombatant.selectedMove = -1;
        enemyCombatant.selectedMove = -1;

        Move1.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            userCombatant.creatureInfo.GetMove(0).Name;
        Move2.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            userCombatant.creatureInfo.GetMove(1).Name;
        Move3.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            userCombatant.creatureInfo.GetMove(2).Name;
        Move4.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text =
            userCombatant.creatureInfo.GetMove(3).Name;

        state = BattleState.prep;

        Content.SetActive(true);
        SelectionsMenu.SetActive(true);
        MoveMenu.SetActive(false);

        OnBattleStart?.Invoke();
    }

    private void OpenMoveMenu()
    {
        InfoBox.text = "Select a move to use, then hit the attack button.";
        MoveMenu.SetActive(true);
        SelectionsMenu.SetActive(false);
        // set all other relative menus to false
    }

    private void StartBattlePhase()
    {
        if (userCombatant.selectedMove != -1)
            StartCoroutine(AttackPhase());
        else
            InfoBox.text = "Select a move first!";
    }

    private void BackToBattlePanel()
    {
        SelectionsMenu.SetActive(true);
        MoveMenu.SetActive(false);
        InfoBox.text = "What will you do?";
        // set all other relative menus to false
    }

    private void SelectMove(int _index)
    {
        userCombatant.selectedMove = _index;
        InfoBox.text = userCombatant.creatureInfo.GetMove(_index).Description;
    }
    private void SelectMove1() { SelectMove(0); }
    private void SelectMove2() { SelectMove(1); }
    private void SelectMove3() { SelectMove(2); }
    private void SelectMove4() { SelectMove(3); }

    private IEnumerator AttackPhase()
    {
        yield return null;
        state = BattleState.fighting;
        enemyCombatant.selectedMove = 0; // degubbing test
        MoveMenu.SetActive(false);
        SelectionsMenu .SetActive(false);

        if (userCombatant.creatureInfo.SpeedStat >
            enemyCombatant.creatureInfo.SpeedStat)
        {
            // user go first
            yield return StartCoroutine(Attack(userCombatant, enemyCombatant));
            if (!enemyCombatant.fainted)
                yield return StartCoroutine(Attack(enemyCombatant, userCombatant));
        }
        else if (userCombatant.creatureInfo.SpeedStat <
            enemyCombatant.creatureInfo.SpeedStat)
        {
            // opponent go first
            yield return StartCoroutine(Attack(enemyCombatant, userCombatant));
            if (!userCombatant.fainted)
                yield return StartCoroutine(Attack(userCombatant, enemyCombatant));
        }
        else
        {
            // speed tie pick random
            if (UnityEngine.Random.Range(0, 2) == 0) // select 0 or 1
            {
                // user go first
                yield return StartCoroutine(Attack(userCombatant, enemyCombatant));
                if (!enemyCombatant.fainted)
                    yield return StartCoroutine(Attack(enemyCombatant, userCombatant));
            }
            else
            {
                // opponent go first
                yield return StartCoroutine(Attack(enemyCombatant, userCombatant));
                if (!userCombatant.fainted)
                    yield return StartCoroutine(Attack(userCombatant, enemyCombatant));
            }
        }

        userCombatant.selectedMove = -1;
        enemyCombatant.selectedMove = -1;
        if (userCombatant.fainted)
        {
            Debug.Log("enemy won");
            StartCoroutine(BattleEnd(false));
        }
        else if (enemyCombatant.fainted)
        {
            Debug.Log("user won");
            StartCoroutine(BattleEnd(true));
        }
        else
        {
            BackToBattlePanel();

            state = BattleState.prep;
        }
        
    }

    private IEnumerator Attack(Combatant _user, Combatant _target)
    {
        yield return null;
        float damageToDeal = 0;
        MoveSO tmp = _user.creatureInfo.GetMove(_user.selectedMove);
        
        damageToDeal = tmp.Power * 
            (_user.creatureInfo.AttackStat / _target.creatureInfo.DefenseStat);
        
        InfoBox.text = _user.creatureInfo.Name + " used " + tmp.Name;
        
        yield return StartCoroutine(ModifyHp(_target, damageToDeal * -1));
        Debug.Log("end modify HP");

        //InfoBox.text = "It was super effective";
        //InfoBox.text = "It hit!";
        //InfoBox.text = "It wasn't very effective";

    }

    public IEnumerator ModifyHp(Combatant _target, float _value)
    {
        var startingHp = _target.hp;
        if (_target.hp + _value >= _target.creatureInfo.MaxHp)
            _target.hp = _target.creatureInfo.MaxHp;
        else if (_target.hp + _value <= 0)
        {
            _target.hp = 0;
            _target.fainted = true;
        }
        else
        {
            _target.hp += _value;
        }

        yield return StartCoroutine(AnimateHealthBar(startingHp, _target, _target.ally));
        Debug.Log("end animate healthbar");
        //if (_target.ally)
        //{
        //    userHealthBar.fillAmount = _target.hp / _target.creatureInfo.MaxHp;
        //}
        //else
        //{
        //    enemyHealthBar.fillAmount = _target.hp / _target.creatureInfo.MaxHp;
        //}
    }

    public IEnumerator AnimateHealthBar(float _start, Combatant _target, bool _isAlly)
    {
        var newHp = _start;
        yield return null;
        while (Mathf.Abs(Mathf.Abs(newHp) - Mathf.Abs(_target.hp)) > 0.1f)
        {
            if (_isAlly)
                userHealthBar.fillAmount  = newHp / _target.creatureInfo.MaxHp;
            else
                enemyHealthBar.fillAmount = newHp / _target.creatureInfo.MaxHp;

            newHp = Mathf.Lerp(newHp, _target.hp, 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        Debug.Log("end animate hp");

        if (_isAlly)
        {
            userHealthBar.fillAmount  = _target.hp / _target.creatureInfo.MaxHp;
        }
        else
        {
            enemyHealthBar.fillAmount = _target.hp / _target.creatureInfo.MaxHp;
        }
    }

    private IEnumerator BattleEnd(bool _playerWon)
    {
        if (_playerWon)
        {
            enemyUI.SetActive(false);
            InfoBox.text = enemyCombatant.creatureInfo.name + " won the battle!";
        }
        else
        {
            userUI.SetActive(false);
            InfoBox.text = "You lost the battle to " + enemyCombatant.creatureInfo.name + "!";
        }
        yield return new WaitForSeconds(1f);

        OnBattleStop?.Invoke();
    }
}

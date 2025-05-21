using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUIContainer spelluicontainer;

    public Class playerclass;
    public float speed = 5;

    public Unit unit;

    private Vector2 movement;
    private Vector2 aim;
    private bool casting;

    public List<Relic> relics;

    private float timeStill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;

        // Initialize spellcaster
        spellcaster = gameObject.AddComponent<SpellCaster>();
        spellcaster.team = Hittable.Team.PLAYER;

        // Scale mana values with wave
        UpdatePlayerStats();
        
        // Set initial spell
        SpellBuilder.Initialize(Resources.Load<TextAsset>("spells").text);
        spellcaster.AddSpell(SpellBuilder.Build("arcane_bolt", spellcaster));
        spellcaster.spell = spellcaster.GetSpell(0);

        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // initialize relics
        relics = new List<Relic>();

        // set ui elements
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
    }

    public void StartLevel()
    {
        UpdatePlayerStats();
    }

void UpdatePlayerStats()
{
    int wave = GameManager.Instance.wave;
    
    // Nullcheck
    if (playerclass == null)
    {
        Debug.LogWarning("No player class selected! Using default values.");
        return;
    }
    
    // Update health for class
    if (hp != null)
    {
        float newHp = RPNEvaluator.EvaluateRPNFloat(playerclass.health, 0, 0, wave);
        hp.SetMaxHP((int)newHp);
    }
    
    // Update mana for class
    spellcaster.max_mana = RPNEvaluator.EvaluateRPNFloat(playerclass.mana, 0, 0, wave);
    spellcaster.mana = spellcaster.max_mana;
    
    // set mana regen
    spellcaster.mana_regen = 10;
    
    // Update spell power for class
    spellcaster.power = (int)RPNEvaluator.EvaluateRPNFloat(playerclass.spellpower, 0, 0, wave);
    
    // Update movement speed for class
    float newSpeed = RPNEvaluator.EvaluateRPNFloat(playerclass.speed, 0, 0, wave);
    speed = newSpeed;

    // Update mana regen for class
    spellcaster.mana_regen = RPNEvaluator.EvaluateRPNFloat(playerclass.mana_regeneration, 0, 0, wave);
}

    // Update is called once per frame
    void Update()
    {
        // Movement
        transform.Translate(movement * speed * Time.deltaTime);
        // keep track of if you haven't been moving
        if (movement.magnitude > 0)
        {
            if (timeStill > 0)
            {
                timeStill = 0;
                EventBus.Instance.DoMove();
            }
        } else
        {
            timeStill += Time.deltaTime;
            EventBus.Instance.DoStandStill((int) timeStill);
        }

        // Switch active spell
        SwitchSpell();

        // Cast spell on left click
        if (Input.GetMouseButtonDown(0) && spellcaster != null)  // Changed from GetMouseButton to GetMouseButtonDown
        {
            // Get mouse position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;  // Keep it in 2D plane
            spellcaster.Cast(transform.position, mousePos);
        }
    }
    void SwitchSpell()
    { // theres probably a nicer way to format this. like a switch statement -kirsten
        int switchto = -1;
        if(Input.GetKeyDown(KeyCode.Alpha1) && spellcaster.GetSpellCount() > 0)
        {
            switchto = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && spellcaster.GetSpellCount() > 1)
        {
            switchto = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && spellcaster.GetSpellCount() > 2)
        {
            switchto = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4) && spellcaster.GetSpellCount() > 3)
        {
            switchto = 3;
        }
        if (switchto >= 0)
        {
            if (spellcaster.spell != spellcaster.GetSpell(switchto)) EventBus.Instance.DoSpellSwitch();
            spellcaster.spell = spellcaster.GetSpell(switchto);
            spelluicontainer.SwitchHighlight(switchto);
        }
    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
        EventBus.Instance.DoMove();
    }

    public void OnAim(InputValue value)
    {
        aim = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        casting = value.isPressed;
    }

    void Die()
    {
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
    }

    public void PickupRelic(Relic r)
    {
        relics.Add(r);
        EventBus.Instance.PickupRelic(r);
    }
}

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

    public float speed = 5;

    public Unit unit;

    private Vector2 movement;
    private Vector2 aim;
    private bool casting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;

        // Initialize spellcaster
        spellcaster = gameObject.AddComponent<SpellCaster>();
        spellcaster.team = Hittable.Team.PLAYER;

        // Scale mana values based on wave (from requirements)
        UpdatePlayerStats();
        
        // Set initial spell
        SpellBuilder.Initialize(Resources.Load<TextAsset>("spells").text);
        spellcaster.AddSpell(SpellBuilder.Build("arcane_bolt", spellcaster));
        spellcaster.spell = spellcaster.GetSpell(0);

        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
    }

    // Add this method back for EnemySpawner
    public void StartLevel()
    {
        UpdatePlayerStats();
    }

    void UpdatePlayerStats()
    {
        int wave = GameManager.Instance.wave;
        // From requirements:
        // Player hp to "95 wave 5 * +"
        if (hp != null)
        {
            float newHp = RPNEvaluator.EvaluateRPNFloat("95 wave 5 * +", 0, 0, wave);
            hp.SetMaxHP((int)newHp);
        }
        // Player mana to "90 wave 10 * +"
        spellcaster.max_mana = RPNEvaluator.EvaluateRPNFloat("90 wave 10 * +", 0, 0, wave);
        spellcaster.mana = spellcaster.max_mana;
        // Player mana regeneration to "10 wave +"
        spellcaster.mana_regen = RPNEvaluator.EvaluateRPNFloat("10 wave +", 0, 0, wave);
        // Player spell power to "wave 10 *"
        spellcaster.power = (int)RPNEvaluator.EvaluateRPNFloat("wave 10 *", 0, 0, wave);
        // Player speed to "5"
        speed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        transform.Translate(movement * speed * Time.deltaTime);

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
        if(Input.GetKeyDown(KeyCode.Alpha1) && spellcaster.GetSpellCount() > 0)
        {
            spellcaster.spell = spellcaster.GetSpell(0);
            spelluicontainer.SwitchHighlight(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && spellcaster.GetSpellCount() > 1)
        {
            spellcaster.spell = spellcaster.GetSpell(1);
            spelluicontainer.SwitchHighlight(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && spellcaster.GetSpellCount() > 2)
        {
            spellcaster.spell = spellcaster.GetSpell(2);
            spelluicontainer.SwitchHighlight(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4) && spellcaster.GetSpellCount() > 3)
        {
            spellcaster.spell = spellcaster.GetSpell(3);
            spelluicontainer.SwitchHighlight(3);
        }
    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
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
}

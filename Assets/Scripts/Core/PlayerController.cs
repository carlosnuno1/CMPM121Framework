using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Hittable hp;
    public RewardScreenManager rewardScreenManager;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spellui;

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
        spellcaster.spell = SpellBuilder.Build("arcane_bolt", spellcaster);

        hp = new Hittable(100, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        rewardScreenManager.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spellui.SetSpell(spellcaster.spell);
    }

    // Add this method back for EnemySpawner
    public void StartLevel()
    {
        UpdatePlayerStats();
    }

    void UpdatePlayerStats()
    {
        // From requirements:
        // Player hp to "95 wave 5 * +"
        // Player mana to "90 wave 10 * +"
        // Player mana regeneration to "10 wave +"
        // Player spell power to "wave 10 *"
        // Player speed to "5"
        
        int wave = GameManager.Instance.wave;
        
        if (hp != null)
        {
            float newHp = RPNEvaluator.EvaluateRPNFloat("95 wave 5 * +", 0, 0, wave);
            hp.SetMaxHP((int)newHp);
        }

        spellcaster.max_mana = RPNEvaluator.EvaluateRPNFloat("90 wave 10 * +", 0, 0, wave);
        spellcaster.mana = spellcaster.max_mana;
        spellcaster.mana_regen = RPNEvaluator.EvaluateRPNFloat("10 wave +", 0, 0, wave);
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        transform.Translate(movement * speed * Time.deltaTime);

        // Cast spell on left click
        if (Input.GetMouseButtonDown(0) && spellcaster != null)  // Changed from GetMouseButton to GetMouseButtonDown
        {
            // Get mouse position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;  // Keep it in 2D plane
            spellcaster.Cast(transform.position, mousePos);
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

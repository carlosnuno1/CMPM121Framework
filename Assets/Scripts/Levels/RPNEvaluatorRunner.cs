using UnityEngine;

public class RPNEvaluatorRunner : MonoBehaviour
{
    void Start()
    {
        // Test real spell expressions from spells.json
        TestRPNFloat("20 power 3 / +", 0f, 10, 1);     // arcane_blast damage
        TestRPNFloat("12", 0f, 10, 1);                 // projectile speed
        TestRPNFloat("8 power 5 / +", 0f, 15, 2);      // with different power/wave
        
        // Test wave-scaling expressions
        TestRPNFloat("95 wave 5 * +", 0f, 10, 2);      // HP scaling
        TestRPNFloat("90 wave 10 * +", 0f, 10, 2);     // Mana scaling
        TestRPNFloat("10 wave +", 0f, 10, 2);          // Regen scaling
        TestRPNFloat("wave 10 *", 0f, 10, 2);          // Spell power scaling
    }

    void TestRPN(string expr, int baseval, int power, int wave)
    {
        int result = RPNEvaluator.EvaluateRPN(expr, baseval, wave);
        Debug.Log($"Int RPN: '{expr}' with baseval={baseval}, power={power}, wave={wave} → {result}");
    }

    void TestRPNFloat(string expr, float baseval, int power, int wave)
    {
        float result = RPNEvaluator.EvaluateRPNFloat(expr, baseval, power, wave);
        Debug.Log($"Float RPN: '{expr}' with baseval={baseval}, power={power}, wave={wave} → {result}");
    }
}
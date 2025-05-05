using UnityEngine;
using System.Collections.Generic;

public class RPNEvaluator
{
    public static int EvaluateRPN(string expr, int baseval, int wave)
    {
        // initialize empty stack
        Stack<int> s = new Stack<int>();
        s.Push(baseval);  // Push the base value first!
        
        // split expression into tokens, and iterate over them
        string[] tokens = expr.Split(' ');
        int a, b;
        foreach (var token in tokens)
        {
            switch(token)
            {
                case "+":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a + b);
                    break;
                case "-":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a - b);
                    break;
                case "*":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a * b);
                    break;
                case "/":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a / b);
                    break;
                case "%":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a % b);
                    break;
                case "wave":
                    s.Push(wave);
                    break;
                default:
                    if (int.TryParse(token, out int val))
                    {
                        s.Push(val);
                    }
                    break;
            }
        }
        return s.Pop();
    }

    /// <summary>
    /// Evaluates an RPN expression, returning a float result.
    /// Supports operators: + - * / %
    /// Variables: power, wave
    /// </summary>
    public static float EvaluateRPNFloat(string expr, float baseval, int power, int wave)
    {
        Stack<float> s = new Stack<float>();
        s.Push(baseval);  // Push the base value first!
        
        string[] tokens = expr.Split(' ');
        float a, b;
        foreach (var token in tokens)
        {
            switch(token)
            {
                case "+":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a + b);
                    break;
                case "-":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a - b);
                    break;
                case "*":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a * b);
                    break;
                case "/":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a / b);
                    break;
                case "%":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a % b);
                    break;
                case "power":
                    s.Push(power);
                    break;
                case "wave":
                    s.Push(wave);
                    break;
                default:
                    if (float.TryParse(token, out float val))
                    {
                        s.Push(val);
                    }
                    break;
            }
        }
        return s.Pop();
    }
}
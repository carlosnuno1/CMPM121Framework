using UnityEngine;
using System.Collections.Generic;

public class RPNEvaluator
{
    public static int EvaluateRPN(string expr, int baseval, int wave)
    {
        // initialize empty stack
        Stack<int> s = new Stack<int>();
        // split expression into tokens, and iterate over them
        string[] tokens = expr.Split(' ');
        int a,b,val = 0;
        foreach (var token in tokens)
        {
            switch(token)
            {
            // if token is an operator, pop 2 values and apply the operator, pushing the result
                case "+":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a+b);
                    break;
                case "-":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a-b);
                    break;
                case "*":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a*b);
                    break;
                case "/":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a/b);
                    break;
                case "%":
                    b = s.Pop();
                    a = s.Pop();
                    s.Push(a%b);
                    break;
            // if token is a value, push to stack
                case "base":
                    s.Push(baseval);
                    break;
                case "wave":
                    s.Push(wave);
                    break;
                default:
                    if (int.TryParse(token, out val)) s.Push(val);
                    break;
            }
        }
        // pop and return the final value
        return s.Pop();
    }
}
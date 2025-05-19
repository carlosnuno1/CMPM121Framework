using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class RelicBuilder
{
    private static List<Relic> relic_types;
    private static bool init = false;

    private static void Initialize()
    {
        relic_types = new List<Relic>();
        var relictext = Resources.Load<TextAsset>("relics");
        JToken jo = JToken.Parse(relictext.text);
        foreach (var relic in jo)
        {
            Relic obj = relic.ToObject<Relic>();
            relic_types.Add(obj);
        }
        init = true;
        EventBus.Instance.OnRelicPickup += OnRelicPickup;
    }


    public static Relic[] GenerateRelics()
    {
        // drop 3 relics at random, no duplicates of ones the player already has
        if (!init) Initialize();
        Relic[] gen = new Relic[3];
        if (relic_types.Count > 0)
        {
            for (var i = 0; i < 3; i++)
            {
                int index = UnityEngine.Random.Range(0, relic_types.Count);
                gen[i] = relic_types[index];
            }
        }
        return gen;
    }

    private static void OnRelicPickup(Relic r)
    {
        relic_types.Remove(r);
    }
}
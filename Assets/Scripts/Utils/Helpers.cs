using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static bool CompareLayer(this GameObject go, int layer) => ((layer & 1 << go.layer) == 1 << go.layer);
}

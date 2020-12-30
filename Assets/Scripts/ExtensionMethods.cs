using System.Collections.Generic;
using UnityEngine;
public static class ExtensionMethods
{
    /// <summary>
    /// Belirli bir aralıktaki değeri, başka belirli aralıktaki değerde orantısal(lineer) olarak nereye karşılık geldiğini bulur, ve o aralık cinsinden çevirir
    /// </summary>
    /// <param name="value"></param>
    /// <param name="from1"></param>
    /// <param name="to1"></param>
    /// <param name="from2"></param>
    /// <param name="to2"></param>
    /// <returns></returns>
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

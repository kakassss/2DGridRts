using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Grid.Utils
{
    /// <summary>Grid hücreleri üzerinde çalışan genel yardımcı metotlar.</summary>
    public static class GridCellUtils
    {
        /// <summary>
        /// Sol alt hücre (origin) ve boyuttan, dikdörtgenin kapladığı tüm hücreleri buffer'a doldurur.
        /// Buffer önce temizlenir, böylece aynı liste tekrar kullanılabilir (allocation yok).
        /// </summary>
        public static void FillRect(Vector2Int origin, Vector2Int size, List<Vector2Int> buffer)
        {
            buffer.Clear();

            for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                buffer.Add(new Vector2Int(origin.x + x, origin.y + y));
        }
    }
}
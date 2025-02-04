using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "StackRun/Level", order = 1)]
    public class LevelData : ScriptableObject
    {
        public List<BlockType> blocks = new();
    }

    public enum BlockType
    {
        Default,
        Star,
        Coin,
        Gem
    }
}

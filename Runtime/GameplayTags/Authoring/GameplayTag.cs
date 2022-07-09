using UnityEngine;

namespace GameplayAbilitySystem.GameplayTags.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Tag")]
    public class GameplayTag : ScriptableObject
    {
        [field: SerializeField] public GameplayTag Parent { get; set; }

        /// <summary>
        /// <para>Check is this gameplay tag is a descendant of another gameplay tag.</para>
        /// By default, only 4 levels of ancestors are searched.
        /// </summary>
        /// <param name="other">Ancestor gameplay tag</param>
        /// <param name="searchLimit">Search limit</param>
        /// <returns>True if this gameplay tag is a descendant of the other gameplay tag</returns>
        public bool IsDescendantOf(GameplayTag other, int searchLimit = 4)
        {
            var i = 0;
            var tag = Parent;
            while (searchLimit > i++)
            {
                // tag will be invalid once we are at the root ancestor
                if (!tag) return false;

                // Match found, so we can return true
                if (tag == other) return true;

                // No match found, so try again with the next ancestor
                tag = tag.Parent;
            }

            // If we've exhausted the search limit, no ancestor was found
            return false;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Spikescape.Leaderboard
{
    public class NameFilter : MonoBehaviour
    {
        public static NameFilter Instance;

        public bool IsReady { get; private set; }

        [SerializeField] private string bannedWordsFileName = "en-bad-words";

        private HashSet<string> _bannedWords;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (IsReady) return;

            _ = LoadBannedWordsAsync();
        }

        public bool IsNameValid(string playerName)
        {
            if (!IsReady)
            {
                Debug.LogError("NameFilter accessed before it is ready.");
                throw new System.InvalidOperationException("NameFilter accessed before it is ready.");
            }

            var nameWords = playerName
                .ToLowerInvariant()
                .Split(new[] { ' ', '-', '_'}, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim());

            foreach (var word in nameWords)
            {
                if (_bannedWords.Contains(word))
                {
                    return false;
                }
            }
            return true;
        }

        private async Task LoadBannedWordsAsync()
        {
            TextAsset file = Resources.Load<TextAsset>(bannedWordsFileName);

            if (file != null)
            {
                var words = file.text
                    .Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => w.Trim().ToLowerInvariant());
                _bannedWords = new HashSet<string>(words);
            }
            else
            {
                Debug.LogError($"Failed to load banned words file. File {bannedWordsFileName}.txt not found.");
                _bannedWords = new HashSet<string>();
            }

            IsReady = true;
            await Task.Yield();
        }
    }
}
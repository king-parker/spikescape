using UnityEngine;

namespace Spikescape.UI.Leaderboard
{
    public class LeaderboardEntryController : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI rankText;
        [SerializeField] private TMPro.TextMeshProUGUI nameText;
        [SerializeField] private TMPro.TextMeshProUGUI scoreText;

        private int _rank;
        private string _name;
        private int _score;

        public int Rank
        {
            get => _rank;
            set
            {
                _rank = value;
                rankText.text = _rank.ToString();
            }
        }

        public string PlayerName
        {
            get => _name;
            set
            {
                _name = value;
                nameText.text = _name;
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                scoreText.text = _score.ToString();
            }
        }

        private void Awake()
        {
            _rank = 100;
            _name = "Player";
            _score = 20;

            rankText.text = _rank.ToString();
            nameText.text = _name;
            scoreText.text = _score.ToString();
        }
    }
}
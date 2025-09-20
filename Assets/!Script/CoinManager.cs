using UnityEngine;
using TMPro;

namespace FlamingOrange
{
    public class CoinManager : MonoBehaviour
    {
        public int coins = 0;
        [SerializeField] private TextMeshProUGUI coinText;

        public int Coins
        {
            get { return coins; }
            private set { coins = Mathf.Max(0, value); }
        }
        public void AddCoins(int amount)
        {
            Coins += amount;
            coinText.text = Coins.ToString();
        }
        public void RemoveCoins(int amount)
        {
            Coins -= amount;
            coinText.text = Coins.ToString();
        }
        void Start()
        {
            coinText.text = Coins.ToString();
        }
    }
}


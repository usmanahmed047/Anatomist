using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Anatomist
{
    public class Screen_Leaderboard : Screen
    {
        [SerializeField]
        Text title, yourScore;


        public override void Open(params object[] args)
        {
            base.Open(args);

            Localize();
        }

        void Localize()
        {
            title.text = LocalizationManager.localization.Leaderboard.ToUpperInvariant();
            yourScore.text = LocalizationManager.localization.YourScore.ToUpperInvariant();
        }

        public override void Close()
        {
            base.Close();
        }
        public override void CloseImmediate()
        {
            base.CloseImmediate();
        }
    }
}
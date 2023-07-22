using UnityEngine;

namespace GamePlayLogic
{
    public class ScoreHandler
    {
        private int _score;
        
        public int Score => _score;

        public ScoreHandler()
        {
            _score = 0;
        }

        public void AddScore(int score)
        {
            _score += score;
        }


        public void ResetScore()=>
            _score = 0;
    }
}
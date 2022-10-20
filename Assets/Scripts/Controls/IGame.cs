using Morkwa.Test.Data;
using Morkwa.Test.Mechanics.Audio;
using Morkwa.Test.Mechanics.Characters;
using UnityEngine;

namespace Morkwa.Test.Mechanics
{
    public interface IGame
    {
        GameConfiguration GameConfiguration { get; }
        Player Player { get; }

        void Construct(SettingGame settingGame);
        void GameOver(bool isWin);
        AudioManager GetAudioManager();
        Transform GetObjectAreFollowing();
        bool GetPlayGameStatus();
        Spawner GetSpawner();
        void SetEnemy(Enemy enemy);
        void SetPlayer(Player player);
        void StartGame();
    }
}
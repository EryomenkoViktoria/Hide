using System;
using Morkwa.Test.Camera;
using Morkwa.Test.Mechanics.Audio;
using Morkwa.Test.UI;
using Morkwa.Test.Mechanics;

namespace Morkwa.Test.Data
{
    [Serializable]
    public class SettingGame
    {
        public Game Game;
        public Spawner Spawner;
        public AudioManager AudioManager;
        public UIManager UIManager;
        public CameraControl CameraControl;
    }
}
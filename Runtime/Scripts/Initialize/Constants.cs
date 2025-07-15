namespace SAGE.Framework.Core
{
    public class Constants
    {
        public const string EmptyChangName = "Display name cannot be empty.";
        public const string InvalidDisplayName = "Invalid display name!";
        public const string NoInternet = "No internet connection.";
        public const string NoRewardAd = "No reward ad available.";
        public const string NoTapjoy = "No Tapjoy ad available.";
        public const string NoFyberOfferWallAds = "No Fyber offer wall ad available.";
        
        public const int TimeFreeze = 20;
        public const int TimeAdd = 30;
        
        public const string LackOfCoinTitle = "Lack of coins";
        public const string LackOfGemTitle = "Lack of gems";
        public const string OutOfCoinContent = "Not enough coin.\nPlay to earn more coins";
        public const string OutOfGemContent = "Not enough gems.\nGo to shop earn more gems?";
        public const string FailByBombContent = "A bomb has been exploded";
        public const string FailContent = "You've ran out of limited time.";
        public const string TimeFreezerAlreadyActive = "Time freezer is already active.";
        
        public const string TimeOutTitle = "Time out";
        public const string TimeOutContent = "You have run out of time.\nDo you want to continue?";
        public const string OutOfSpaceTitle = "Out of space";
        public const string OutOfSpaceContent = "You have run out of space.\nUse the item to continue?";
        public const string OverSpaceTitle = "Over space";
        public const string OverSpaceContent = "You're stuck.\nUse the item to continue?";
    }
    
    public class PrefKey
    {
        public const string EnableMusic = "EnableMusic";
        public const string EnableSound = "EnableSound";
    }

    public class SoundKey
    {
        public const string MainMenu = "MainMenu";
        public const string Gameplay = "Gameplay";
        public const string WoodenBlocksCollider = "WoodenBlocksCollider";
        public const string TouchDown = "TouchDown";
        public const string TouchUp = "TouchUp";
        public const string HammerHit = "HammerHit";
        public const string Timing = "Timing";
        public const string Win = "Win";
        public const string Lose = "Lose";
        public const string BlockMove = "BlockMove";
        public const string IceBreak = "IceBreak";
        public const string CollectCoin = "CollectCoin";
        public const string Firework_1 = "Firework_1";
        public const string Firework_2 = "Firework_2";
        public const string Unlock = "Unlock";
    }

    public class PoolKey
    {
        public const string CollectWare = "CollectWare";
        public const string HammerVFX = "HammerVFX";
        public const string SmashVFX = "SmashVFX";
        public const string BombExplosionVFX = "BombExplosionVFX";
        public const string ChargeExplosionVFX = "ChargeExplosionVFX";
    }
    
    public enum TypeBooster
    {
        None,
        Hammer,
        TimeFreezer,
        Shuffle,
        Magnet,
    }

    public enum GameState
    {
        Ready,
        Playing,
        Pause,
        SelectBooster,
        Win,
        Lose
    }

    public struct KeyEvent
    {
        public const string BrokenKey = "BrokenKey";
        public const string BrokenBlockByGate = "BrokenBlockByGate";
        public const string BrokenScrewDriver = "BrokenScrewDriver";
        public const string ActiveBooster = "ActiveBooster";
        public const string OnSliderValueChanged = "OnSliderValueChanged";
        public const string OnSliderValueChangedWithMultiple = "OnSliderValueChangedWithMultiple";
    }
}
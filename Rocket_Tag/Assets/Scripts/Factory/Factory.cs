using UnityEngine;
internal interface IFactory    //  ゲームマネージャーのファクトリー
{
    public GameManager CreateGameMgr();
    public TimeManager CreateTimeMgr();
}
internal class RealFactory : IFactory
{
    public GameManager CreateGameMgr()
    {
        return new GameManager();
    }
    public TimeManager CreateTimeMgr()
    {
        return new TimeManager();
    }
}
//internal class RealTimeMgrFactory : IFactory
//{
//    public GameManager CreateGameMgr()
//    {
//        return new TimeManager();
//    }
//}
//IEffectLoaderFactory factory = new DefaultEffectLoaderFactory();
//loader = factory.Create();
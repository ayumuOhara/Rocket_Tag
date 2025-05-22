using UnityEngine;
internal interface IGameMgrFactory    //  ゲームマネージャーのファクトリー
{
    public GameManager CreateGameMgr();
}
internal class RealGameMgrFactory : IGameMgrFactory
{
    public GameManager CreateGameMgr()
    {
        return new GameManager();
    }
}
//IEffectLoaderFactory factory = new DefaultEffectLoaderFactory();
//loader = factory.Create();
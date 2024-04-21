using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Util
{
    public class DBLoader : Singleton<DBLoader>
    {
        public void LoadData()
        {
            // TODO: Connect to DB
            UserManager.Instance.userConfig.leftInputPerformance[MiniGame.Cycle] = 20f;
        }
    }

}

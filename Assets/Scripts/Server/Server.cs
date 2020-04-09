using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace SightGame.Server
{
    public class Server
    {
        private static Server _instance;

        public static Server Instance => _instance ?? (_instance = new Server());

        public ServerData ServerData { get; set; }

        public void Init()
        {
            ServerData = new ServerData();
        }

        public void SetAllPictureIds()
        {
            const int wholeSpritesLen = 34;

            for (int i = 0; i < wholeSpritesLen; ++i)
            {
                ServerData.allPics.ids.Add(i);
            }
        }

        public void RequestAlbume(int difficulty)
        {
            int picsCount = 0;
            if (difficulty == 1)
                picsCount = 4;
            if (difficulty == 2)
                picsCount = 7;
            if (difficulty == 3)
                picsCount = 10;


            ServerData.albume = new AlbumeModel();

            var random = new Random();

            var allPicIds = ServerData.allPics.ids.ToList();

            {
                int commonIndex = random.Next(0, allPicIds.Count);
                ServerData.albume.commonPicId = allPicIds[commonIndex];
                allPicIds.RemoveAt(commonIndex);
            }

            for (int i = 0; i < picsCount - 1; ++i)
            {
                int index = random.Next(0, allPicIds.Count);
                int id = allPicIds[index];
                ServerData.albume.topPage.picIds.Add(id);
                allPicIds.RemoveAt(index);
            }

            for (int i = 0; i < picsCount - 1; ++i)
            {
                int index = random.Next(0, allPicIds.Count);
                int id = allPicIds[index];
                ServerData.albume.bottomPage.picIds.Add(id);
                allPicIds.RemoveAt(index);
            }
        }
    }
}
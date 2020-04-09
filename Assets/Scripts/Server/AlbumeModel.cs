using System;
using System.Collections;
using System.Collections.Generic;

namespace SightGame.Server
{
    public class AlbumeModel
    {
        public int commonPicId = -1;
        
        public AlbumePage topPage = new AlbumePage();
        public AlbumePage bottomPage = new AlbumePage();}

    public class AlbumePage
    {
        public List<int> picIds = new List<int>();
        public List<int> cellRots = new List<int>();
    }
}
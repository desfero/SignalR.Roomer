using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Roomer.Model;

namespace TicTacToeWeb.Roomer
{
    public class RoomHashSet : IEnumerable<Room>
    {
        private readonly HashSet<Room> _list;

        protected HashSet<Room> InnerList
        {
            get { return _list; }
        }

        public RoomHashSet()
        {
            _list = new HashSet<Room>();
        }

        public Room this[string roomName]
        {
            get
            {
                var room = InnerList.FirstOrDefault(r => r.Name == roomName);

                return room;
            }
        }

        /// <summary>
        /// Check if room exist
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns>Return true if room exist</returns>
        public bool Contains(string roomName)
        {
            var room = InnerList.FirstOrDefault(r => r.Name == roomName);

            return room != null;
        }

        /// <summary>
        /// Remove room
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns>Return true if room existed before, otherwise false</returns>
        public bool Remove(string roomName)
        {
            var room = InnerList.FirstOrDefault(r => r.Name == roomName);

            if (room != null)
            {
                InnerList.Remove(room);

                return true;
            }

            return false;
        }

        public int Count
        {
            get { return InnerList.Count; }
        }

        public void CreateRoom(string roomName, string password)
        {
            InnerList.Add(new Room(roomName, password));
        }

        public void JoinRoom(string roomName, User user)
        {
            this[roomName].JoinRoom(user);
        }

        public void LeaveRoom(string roomName, string userConnectionId)
        {
            this[roomName].LeaveRoom(userConnectionId);
        }

        public IEnumerator<Room> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Roomer.Model
{
    public class Room
    {
        private readonly string _password;

        public Room(string roomName, string password = null)
        {
            Name = roomName;
            if (!String.IsNullOrEmpty(_password))
            {
                _password = Hash(password);  
            }

            Users = new HashSet<User>();
        }

        public string Name { get; set; }
        public HashSet<User> Users { get; set; }

        public User FirstInRoom
        {
            get { return Users.First(); }
        }

        public bool IsLocked { get { return !String.IsNullOrWhiteSpace(_password); } }
        public bool IsEmpty { get { return Users.Count == 0; } }

        public void JoinRoom(User user)
        {
            Users.Add(user);
        }

        public void LeaveRoom(string connectionId)
        {
            Users.RemoveWhere(u => u.ConnectionId == connectionId);
        }

        public bool CheckPassword(string password)
        {
            var hashPass = Hash(password);

            return _password == hashPass;
        }

        #region Overrides
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Room);
        }

        public bool Equals(Room room)
        {
            return room != null && Name.Equals(room.Name);
        }
        #endregion

        private static string Hash(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}

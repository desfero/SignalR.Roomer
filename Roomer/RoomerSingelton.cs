using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Roomer.Model;
using TicTacToeWeb.Roomer;

namespace Roomer
{
    public sealed class RoomerSingelton
    {
        // Singleton instance for different hubs
        private static Dictionary<string, Lazy<RoomerSingelton>> dictionary = new Dictionary<string, Lazy<RoomerSingelton>>();

        public RoomHashSet Rooms;

        private RoomerSingelton(IHubContext context)
        {
            Clients = context.Clients;

            Rooms = new RoomHashSet();
        }

        private IHubConnectionContext<dynamic> Clients { get; set; }

        public static RoomerSingelton Instance(string hubName)
        {
            if(!dictionary.ContainsKey(hubName))
                dictionary[hubName] = new Lazy<RoomerSingelton>(() => new RoomerSingelton(GlobalHost.ConnectionManager.GetHubContext(hubName)));

            return dictionary[hubName].Value;
        }

        internal void JoinRoom(string roomName, User user)
        {
            Rooms.JoinRoom(roomName, user);
        }

        internal void CreateRoom(string roomName, string password)
        {
            Rooms.CreateRoom(roomName, password);
        }

        internal void LeaveRoom(string roomName, string connectionId)
        {
            var user = Rooms[roomName].Users.Single(u => u.ConnectionId == connectionId);

            Rooms.LeaveRoom(roomName, connectionId);

            if (Rooms[roomName].IsEmpty)
            {
                Rooms.Remove(roomName);
            }
            else
            {
                Clients.Clients(Rooms[roomName].Users.Select(u => u.ConnectionId).ToList()).R_UserLeaveRoom(user);
            }
        }

        public dynamic AllInRoomExcept(string roomName, string connectionId)
        {
            var clients = Rooms[roomName].Users.Where(u => u.ConnectionId != connectionId).Select(u => u.ConnectionId);

            return Clients.Clients(clients.ToArray());
        }

    }
}
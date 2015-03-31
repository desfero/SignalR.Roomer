using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Roomer.Model;

namespace Roomer
{
    public enum R_JoinRoomResult
    {
        Joined,
        CreatedAndJoined,
        WrongPassword,
        UserAlreadyExist,
        RoomIsFull
    }

    public abstract class RoomerHub : Hub
    {
        protected readonly RoomerSingelton Roomer;

        // Implement internal properties from Hub class
        protected new IHubCallerConnectionContext<dynamic> Clients { get { return base.Clients; } }
        protected new HubCallerContext Context { get { return base.Context; }}

        protected RoomerHub(RoomerSingelton roomer)
        {
            Roomer = roomer;
        }

        public virtual R_JoinRoomResult R_JoinRoom(string roomName, string userName, string password = "")
        {
            var user = new User(Context.ConnectionId, userName);

            if (Roomer.Rooms.Contains(roomName))
            {
                var room = Roomer.Rooms[roomName];

                if (room.CheckPassword(password))
                {
                    return R_JoinRoomResult.WrongPassword;
                }
                if (room.Users.Contains(user))
                {
                    return R_JoinRoomResult.UserAlreadyExist;
                }

                Roomer.JoinRoom(roomName, user);

                // Send to all clients message that rooms is changed
                Clients.All.R_RoomsChanged(R_GetRooms());

                return R_JoinRoomResult.Joined;
            }

            // Create new room
            Roomer.CreateRoom(roomName, password);
            // Join user to that room
            Roomer.JoinRoom(roomName, user);

            // Send a message to other in group that user was joined to this room
            var otherInRoom = Roomer.Rooms[roomName].Users.Where(u => u.ConnectionId != user.ConnectionId);
            Clients.Clients(otherInRoom.Select(u => u.ConnectionId).ToList()).R_UserJoinRoom(user); // R_UserJoinRoom(user);

            // Send to all clients message that rooms is changed
            Clients.All.R_RoomsChanged(R_GetRooms());

            return R_JoinRoomResult.CreatedAndJoined;
        }

        public virtual void R_LeaveRoom(string roomName)
        {
            Roomer.LeaveRoom(roomName, Context.ConnectionId);

            Clients.All.R_RoomsChanged(R_GetRooms());
        }

        public virtual bool R_IsRoomExist(string roomName)
        {
            return Roomer.Rooms.Contains(roomName);
        }

        public virtual IEnumerable<Room> R_GetRooms()
        {
            return Roomer.Rooms;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var rooms = Roomer.Rooms.Where(r => r.Users.Count(u => u.ConnectionId == Context.ConnectionId) == 1).ToArray();

            for (var i = 0; i < rooms.Count(); ++i)
            {
                R_LeaveRoom(rooms[i].Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}

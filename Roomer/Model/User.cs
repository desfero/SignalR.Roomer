namespace Roomer.Model
{
    public class User
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }

        public User(string connectionId, string name)
        {
            ConnectionId = connectionId;
            Name = name;
        }

        public override int GetHashCode()
        {
            return ConnectionId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User user)
        {
            return user != null && ConnectionId.Equals(user.ConnectionId);
        }
    }
}

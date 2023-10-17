namespace DServCloudStatusCommonClasses
{
    [Serializable]
    public class BotUser
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ChatId { get; set; }
        public bool IsAdmin { get; set; }

        public BotUser()
        {
        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }

        public bool Equals(BotUser other)
        {
            return this.Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is BotUser otherBotUser)
            {
                return Equals(otherBotUser);
            }

            return false;
        }
    }
}
namespace Generated
{
    public class FlowManagementData
    {
        public FlowType FlowType { get; private set; }
        public int FlowOrder { get; private set; }
        public string PlayingBgm { get; private set; }
        public string Title { get; private set; }

        public FlowManagementData(FlowType flowType, int flowOrder, string playingBgm, string title)
        {
            FlowType = flowType;
            FlowOrder = flowOrder;
            PlayingBgm = playingBgm;
            Title = title;
        }
    }
    public class ChatData
    {
        public int FlowOrder { get; private set; }
        public int Order { get; private set; }
        public ChatProfileType ChatProfileType { get; private set; }
        public string Message { get; private set; }
        public string ImageResourceKey { get; private set; }

        public ChatData(int flowOrder, int order, ChatProfileType chatProfileType, string message, string imageResourceKey)
        {
            FlowOrder = flowOrder;
            Order = order;
            ChatProfileType = chatProfileType;
            Message = message;
            ImageResourceKey = imageResourceKey;
        }
    }
    public class ChatProfileData
    {
        public ChatProfileType ChatProfileType { get; private set; }
        public string Name { get; private set; }
        public string IconResourceKey { get; private set; }

        public ChatProfileData(ChatProfileType chatProfileType, string name, string iconResourceKey)
        {
            ChatProfileType = chatProfileType;
            Name = name;
            IconResourceKey = iconResourceKey;
        }
    }
}

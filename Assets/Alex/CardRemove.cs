namespace events
{
    public class CardRemove : CardEvent
    {
        public CardRemove(CardWrapper card) : base(card)
        {
        }
    }
}
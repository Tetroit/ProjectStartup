namespace events
{
    public class CardRelease : CardEvent
    {
        public CardRelease(CardWrapper card) : base(card)
        {
        }
    }
}
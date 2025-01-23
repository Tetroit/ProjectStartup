namespace events
{
    public class CardClick : CardPlayed
    {
        public CardClick(CardWrapper card) : base(card, null)
        {
        }
    }
}
namespace events
{
    public class CardPlayed : CardEvent
    {
        public CardEffect effect;
        public CardPlayed(CardWrapper card, CardEffect effect) : base(card)
        {
            this.effect = effect;
        }
    }
}
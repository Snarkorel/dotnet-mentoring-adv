namespace CartingService.Entities
{
    public class Cart
    {
        public int Id { get; }
        public IEnumerable<CartItem> Items { get; }
    }
}

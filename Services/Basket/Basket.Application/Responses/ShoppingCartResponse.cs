namespace Basket.Application.Responses
{
    public record class ShoppingCartResponse
    {
        public string? UserName { get; set; }
        // List of items in cart
        public List<ShoppingCartItemResponse>? Items { get; set; }

        public ShoppingCartResponse()
        {
            UserName = string.Empty;
            Items = new List<ShoppingCartItemResponse>();
        }
        //Ctor with username only
        // this : Before running MY code, call THIS OTHER constructor first here
        // constrctor 3 is called here !! 
        // this(...) = constructor chaining
        public ShoppingCartResponse(string userName) : this(userName,new List<ShoppingCartItemResponse>())
        {
              
        }
        //Full Ctor
        public ShoppingCartResponse(string userName,List<ShoppingCartItemResponse> items)
        {
            userName = userName ?? string.Empty;
            Items = items?? new List<ShoppingCartItemResponse>();
        }
        // Always UP-TO-DATE! Recalculates from Items every time you ask.
        // Example: (3 × ₹999) + (1 × ₹499) = ₹3,496
        public decimal TotalPrice => Items.Sum(item=>item.Quantity * item.Price);

        // Same thing written the LONG way:
        // public decimal TotalPrice
        // {
        //     get
        //     {
        //         return Items.Sum(item => item.Quantity * item.Price);
        //     }
        // }
    }
}

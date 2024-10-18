namespace Shopping.Web.Pages
{
    public class OrderListModel(IOrderingService orderingService, ILogger<OrderListModel> logger) : PageModel
    {
        public IEnumerable<OrderModel> Orders { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // assumption customerId is passed in from the UI authenticated user Bob
            var customerId = new Guid("e6e99a13-d3a8-4d03-b391-df5d720b2813");

            var response = await orderingService.GetOrdersByCustomer(customerId);
            Orders = response.Orders;

            return Page();
        }
    }
}
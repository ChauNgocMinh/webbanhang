using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.Configuration;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly WebBanHangContext _context;
        private readonly PaymentServices _paymentServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public static IConfiguration Configuration { get; set; }

        public OrderController(ILogger<CartController> logger, WebBanHangContext context, IHttpContextAccessor httpContextAccessor, PaymentServices paymentServices)
        {
            _context = context;
            _logger = logger;
            _paymentServices = paymentServices;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> ProceedPayment(int paymentType, OrderViewModel model, Guid cartId)
        {
            if (paymentType == -1)
            {
                return BadRequest("Payment not selected");
            }

            var infoOrder = new InfoOrder   
            {
                Status = 1,
                PaymentMethod = Convert.ToBoolean(paymentType),
                Name = model.InfoOrder.Name,
                Phone = model.InfoOrder.Phone,
                Email = model.InfoOrder.Email,
                City = model.InfoOrder.City,
                District = model.InfoOrder.District,
                Address = model.InfoOrder.Address,
                Total = model.InfoOrder.Total,
                Date = DateTime.Now,
                OrderItems = model.OrderItem.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductName = item.ProductName,
                    Image = item.Image,
                }).ToList()
            };

            _context.InfoOrders.Add(infoOrder);

            await _context.SaveChangesAsync();

            if (paymentType == 1)
            {
                return Ok(_paymentServices.CreateVNPAYRequestURL(
                    model,
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}{Url.Action("AfterPayment", "Order", new { cartId })!}",
                    "127.0.0.1",
                    "NCB",
                    Guid.NewGuid()
                    )
                );
            }
            else
            {
                return await PaymentWithPaypal(model);
            }
        }
        //Xóa item trong giỏ hàng
        [HttpGet]
        public async Task<IActionResult> AfterPayment(Guid cartId)
        {
            var CartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == cartId);

            if (CartItem != null)
            {
                _context.Remove(CartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index()
        {
            var order = await _context.InfoOrders.Select(o => new InfoOrder
            {
                Id = o.Id,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                Name = o.Name,
                Phone = o.Phone,
                Email = o.Email,
                City = o.City,
                District = o.District,
                Address = o.Address,
                Total = (float)o.Total,
                Date = o.Date,
                OrderItems = o.OrderItems
            }).OrderByDescending(p => p.Date).ToListAsync();
            return View(order);
        }
         public async Task<IActionResult> Detail(Guid Id)
        {
            var order = _context.InfoOrders.Where(x => x.Id == Id).Include(c => c.OrderItems).FirstOrDefault();

            return View(order);
        }


        [NonAction]
        public async Task<IActionResult> PaymentWithPaypal(OrderViewModel model)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            var paypalSettings = Configuration.GetSection("PayPal:Settings");
            string ClientId = paypalSettings["ClientId"];
            string ClientSecret = paypalSettings["ClientSecret"];
            string mode = paypalSettings["Mode"];
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientId, ClientSecret, mode);
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = HttpContext.Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}/User/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, model);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    ISession session = _httpContextAccessor.HttpContext.Session;
                    //string guid = Guid.NewGuid().ToString();

                    session.SetString(guid, createdPayment.id);
                    return Ok(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = HttpContext.Request.Query["guid"];
                    ISession session = _httpContextAccessor.HttpContext.Session;
                    var executedPayment = ExecutePayment(apiContext, payerId, session.GetString(guid));
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return Ok(new { success = false, message = "Đặt hàng không thành công" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Đặt hàng không thành công" });
            }
            //on successful payment, show success page to user.  
            return Ok(new { success = true, message = "Đặt hàng thành công" });
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, OrderViewModel model)
        {
            try
            {
                //create itemlist and add item objects to it  
                var itemList = new ItemList()
                {
                    items = new List<Item>()
                };
                //Adding Item Details like name, currency, price etc  
                itemList.items.Add(new Item()
                {
                    name = "Item Name comes here",
                    currency = "USD",
                    price = ((int)(model.InfoOrder.Total / 24545.5)).ToString(),
                    quantity = "1",
                    sku = "sku"
                });
                var payer = new Payer()
                {
                    payment_method = "paypal"
                };
                // Configure Redirect Urls here with RedirectUrls object  
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = redirectUrl + "&Cancel=true",
                    return_url = redirectUrl
                };
                // Adding Tax, shipping and Subtotal details  
                var details = new Details()
                {
                    tax = "0",
                    shipping = "0",
                    subtotal = ((int)(model.InfoOrder.Total / 24545.5)).ToString(),
                };
                //Final amount with details  
                var amount = new Amount()
                {
                    currency = "USD",
                    total = ((int)(model.InfoOrder.Total / 24545.5)).ToString(),
                    details = details
                };
                var transactionList = new List<Transaction>();
                // Adding description about the transaction  
                var paypalOrderId = DateTime.Now.Ticks;
                transactionList.Add(new Transaction()
                {
                    description = $"Invoice #{paypalOrderId}",
                    invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                    amount = amount,
                    item_list = itemList
                });
                this.payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };
                // Create a payment using a APIContext  
                return this.payment.Create(apiContext);
            }
            catch (Exception ex)
            {
                return this.payment.Create(apiContext);
            }
        }
    }
}

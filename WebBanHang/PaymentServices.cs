using PayPal.Api;
using WebBanHang.ViewModels;

namespace WebBanHang;

public class PaymentServices
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PaymentServices(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public string CreateVNPAYRequestURL(OrderViewModel order, string returnUrl, string clientIpAddress, string bankCode, Guid orderId, string command = "pay", string locale = "vn")
    {
        var vnpaySettingsConfig = _configuration.GetSection("VNPAY");

        //Get Config Info
        string vnp_Returnurl = returnUrl; //URL nhan ket qua tra ve 
        string vnp_Url = vnpaySettingsConfig.GetValue<string>("Url"); //URL thanh toan cua VNPAY 
        string vnp_TmnCode = vnpaySettingsConfig.GetValue<string>("TmnCode"); //Ma định danh merchant kết nối (Terminal Id)
        string vnp_HashSecret = vnpaySettingsConfig.GetValue<string>("HashSecret"); //Secret Key

        VnPayLibrary vnpay = new();

        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", command);
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        var amount = Convert.ToDecimal(order.InfoOrder.Total * 100);
        vnpay.AddRequestData("vnp_Amount", amount.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
        vnpay.AddRequestData("vnp_BankCode", bankCode);

        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", clientIpAddress);

        vnpay.AddRequestData("vnp_Locale", locale);
        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {orderId}");
        vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef", orderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

        return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
    }

    public string CreatePayPalRequestURL(HttpContext httpContext,OrderViewModel orderViewModel)
    {
        var paypalSettingsConfig = _configuration.GetSection("PayPal");

        var ClientID = paypalSettingsConfig.GetValue<string>("Key");
        var ClientSecret = paypalSettingsConfig.GetValue<string>("Secret");
        var mode = paypalSettingsConfig.GetValue<string>("mode");

        APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);

        //this section will be executed first because PayerID doesn't exist  
        //it is returned by the create function call of the payment class  
        // Creating a payment  
        // baseURL is the url on which paypal sendsback the data.
        var baseURI = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/Order/PaymentWithPayPal?";
        //here we are generating guid for storing the paymentID received in session  
        //which will be used in the payment execution  
        var guid = Convert.ToString(new Random().Next(100000));
        //CreatePayment function gives us the payment approval url  
        //on which payer is redirected for paypal account payment  
        var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid, orderViewModel);
        //get links returned from paypal in response to Create function call  
        var links = createdPayment.links.GetEnumerator();
        string? paypalRedirectUrl = null;

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
        ISession session = _httpContextAccessor.HttpContext!.Session;
        //string guid = Guid.NewGuid().ToString();

        session.SetString(guid, createdPayment.id);
        return paypalRedirectUrl!;
    }

    private static Payment CreatePayment(APIContext apiContext, string redirectUrl, OrderViewModel model)
    {
        Payment payment = new();
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
                price = ((int)(model.InfoOrder.Total / 23820)).ToString(),
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
                subtotal = ((int)(model.InfoOrder.Total / 23820)).ToString(),
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = ((int)(model.InfoOrder.Total / 23820)).ToString(),
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
            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return payment.Create(apiContext);
        }
        catch (Exception)
        {
            return payment.Create(apiContext);
        }
    }

    private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
    {
        var paymentExecution = new PaymentExecution()
        {
            payer_id = payerId
        };
        return new Payment()
        {
            id = paymentId
        }.Execute(apiContext, paymentExecution);
    }

}
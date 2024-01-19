using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace WebBanHang;

public class VnPayLibrary
{
    public const string VERSION = "2.1.0";

    private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData.Add(key, value);
        }
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _responseData.Add(key, value);
        }
    }

    public string GetResponseData(string key)
    {
        if (_responseData.TryGetValue(key, out string? retValue))
            return retValue;
        return string.Empty;
    }

    public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
    {
        string queryString = string.Join('&', _requestData.Select(kv =>
        {
            if (string.IsNullOrEmpty(kv.Value))
                return string.Empty;
            return $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}";
        }).Where(s => s != string.Empty));

        baseUrl += $"?{queryString}";

        var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, queryString);

        baseUrl += $"&vnp_SecureHash={vnp_SecureHash}";

        return baseUrl;
    }

    public bool ValidateSignature(string inputHash, string secretKey)
    {
        return Utils.HmacSHA512(secretKey, GetResponseData())
            .Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    private string GetResponseData()
    {
        _responseData.Remove("vnp_SecureHash");
        _responseData.Remove("vnp_SecureHashType");
        return string.Join("&", _responseData.Select(kv =>
        {
            if (string.IsNullOrEmpty(kv.Value))
                return string.Empty;
            return $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}";
        }).Where(s => s != string.Empty));
    }
}

public class Utils
{
    public static string HmacSHA512(string key, string inputData)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        return string.Join(string.Empty, hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData)).Select(@byte => @byte.ToString("x2")));
    }
}

public class VnPayCompare : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null) return -1;
        if (y == null) return 1;
        if (x == y) return 0;

        var vnpCompare = CompareInfo.GetCompareInfo("en-US");

        return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
    }
}
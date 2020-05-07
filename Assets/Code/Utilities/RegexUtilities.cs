/*
    _author: sun hai lang
    _time: 2020-03-25
 */
using System.Text.RegularExpressions;

namespace SoftLiu.Utilities
{
    public static class RegexUtilities
    {
        /// <summary>
        /// 判断是否是地阿奴按号码
        /// </summary>
        /// <param name="phoneNumber">要判断的字符串</param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber.Replace(" ", ""), @"^1(3[0-9]|5[0-9]|7[6-8]|8[0-9])[0-9]{8}$");
        }
        /// <summary>
        /// 是否是IP
        /// </summary>
        /// <param name="str_IP">要判断的字符串</param>
        /// <returns></returns>
        public static bool IPCheck(string str_IP)
        {
            string num = "(25[0-5]|2[0-4]//d|[0-1]//d{2}|[1-9]?//d)";
            return Regex.IsMatch(str_IP, ("^" + num + "//." + num + "//." + num + "//." + num + "$"));
        }
        /// <summary>
        /// 判断是否是电子邮箱
        /// </summary>
        /// <param name="str_Email">判断的字符串</param>
        /// <returns></returns>
        public static bool IsEmail(string str_Email)
        {
            return Regex.IsMatch(str_Email, @"^([/w-/.]+)@((/[[0-9]{1,3}/.[0-9] {1,3}/.[0-9]{1,3}/.)|(([/w-]+/.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(/)?]$");
        }
        /// <summary>
        /// 是否是网址
        /// </summary>
        /// <param name="str_url">判断的网址</param>
        /// <returns></returns>
        public static bool IsUrl(string str_url)
        {
            return Regex.IsMatch(str_url, @"http(s)?://([/w-]+/.)+[/w-]+(/[/w- ./?%&=]*)?");
        }
    }
}

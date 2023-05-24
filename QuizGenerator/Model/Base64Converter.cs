using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Model
{
    class Base64Converter
    {
        static public string Encode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var base64input = Convert.ToBase64String(inputBytes);
            return base64input;
        }
        static public string Decode(string input)
        {
            var base64EncodedInput = Convert.FromBase64String(input);
            var inputString = Encoding.UTF8.GetString(base64EncodedInput);
            return inputString;
        }
    }
}

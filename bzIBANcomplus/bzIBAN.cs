using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using dbIBANcomplus;

[assembly: ApplicationName("bzIBANcomplus")]
[assembly: ApplicationActivation(ActivationOption.Library)]

namespace bzIBANcomplus
{
    [Transaction(TransactionOption.RequiresNew)]
    [ObjectPooling(true, 5, 10)]
    public class bzIBAN : ServicedComponent
    {
        /// <summary>
        /// gets IBAN by Account Number from dbIBAN 
        /// </summary>
        /// <param name="AccNum">account number</param>
        /// <returns>IBAN</returns>
        [ AutoComplete(true) ]
        public string getIBAN(string AccNum)
        {
            string iban = "";
            try
            {
                dbIBAN db = new dbIBAN();
                iban = db.getIBAN(AccNum);
                ContextUtil.SetComplete();
            }
            catch (Exception)
            {
                ContextUtil.SetAbort();
            }            
            return iban;
        }

        /// <summary>
        /// verifies IBAN
        /// </summary>
        /// <param name="iban">IBAN</param>
        /// <returns>checking result (boolean)</returns>
        [ AutoComplete(true) ]
        public bool verifyIBAN(string iban)
        {
            bool res = false;
            string tmpStr = "";
            string convertStr = "";
            //
            try
            {                
                // Check that the total IBAN length is correct as per the country. If not, the IBAN is invalid.
                // total IBAN length must be 24 characters
                if (iban.Length != 24) return false;
                //
                // Move the four initial characters to the end of the string.
                tmpStr = iban.Substring(4, 20).ToUpper() + iban.Substring(0, 4).ToUpper();
                //
                // Replace each letter in the string with two digits, thereby expanding the string, where A = 10, B = 11, ..., Z = 35.
                for (int i=0;i<iban.Length;i++)
                {
                    bool isLetter = !String.IsNullOrEmpty(tmpStr) && Char.IsLetter(tmpStr[i]);
                    if (isLetter)
                    {
                        convertStr += ConvertLetterToDigit(tmpStr[i]);
                    }
                    else
                    {
                        convertStr += tmpStr[i];
                    }
                }
                //
                // Interpret the string as a decimal integer and compute the remainder of that number on division by 97.
                if (CalculateMod97(convertStr) == 1)
                {
                    // If the remainder is 1, the check digit test is passed and the IBAN might be valid.
                    res = true;
                }
                else
                {
                    res = false;
                }
                ContextUtil.SetComplete();
            }
            catch (Exception)
            {
                ContextUtil.SetAbort();
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string ConvertLetterToDigit(char p)
        {
            string convertedLetter = "";
            switch (p)
            {
                // ABCDEFGHIJKLMNOPQRSTUVWXYZ
                case 'A':
                    convertedLetter = "10";
                    break;
                case 'B':
                    convertedLetter = "11";
                    break;
                case 'C':
                    convertedLetter = "12";
                    break;
                case 'D':
                    convertedLetter = "13";
                    break;
                case 'E':
                    convertedLetter = "14";
                    break;
                case 'F':
                    convertedLetter = "15";
                    break;
                case 'G':
                    convertedLetter = "16";
                    break;
                case 'H':
                    convertedLetter = "17";
                    break;
                case 'I':
                    convertedLetter = "18";
                    break;
                case 'J':
                    convertedLetter = "19";
                    break;
                case 'K':
                    convertedLetter = "20";
                    break;
                case 'L':
                    convertedLetter = "21";
                    break;
                case 'M':
                    convertedLetter = "22";
                    break;
                case 'N':
                    convertedLetter = "23";
                    break;
                case 'O':
                    convertedLetter = "24";
                    break;
                case 'P':
                    convertedLetter = "25";
                    break;
                case 'Q':
                    convertedLetter = "26";
                    break;
                case 'R':
                    convertedLetter = "27";
                    break;
                case 'S':
                    convertedLetter = "28";
                    break;
                case 'T':
                    convertedLetter = "29";
                    break;
                case 'U':
                    convertedLetter = "30";
                    break;
                case 'V':
                    convertedLetter = "31";
                    break;
                case 'W':
                    convertedLetter = "32";
                    break;
                case 'X':
                    convertedLetter = "33";
                    break;
                case 'Y':
                    convertedLetter = "34";
                    break;
                case 'Z':
                    convertedLetter = "35";
                    break;
                default:
                    convertedLetter = "00";
                    break;
            }
            return convertedLetter;
        }


        /// <summary>
        /// Calculate mod 97 from input string
        /// </summary>
        /// <param name="convertStr">converted IBAN to digits only</param>
        /// <returns>result of convertStr mod 97</returns>
        private int CalculateMod97(string convertStr)
        {
            int remainder = 0;
            string N = "";
            // 1. Construct N from the first 9 digits of convertStr.
            N = convertStr.Substring(0, 9);
            // 2. Calculate N mod 97.
            remainder = int.Parse(N) % 97;
            // 3. Construct a new 9 digit N from the above result (step 2) followed by the next 7 digits of convertStr.
            N = remainder.ToString() + convertStr.Substring(9, 7);
            // 4. Calculate N mod 97.
            remainder = int.Parse(N) % 97;
            // 5. Construct a new 9 digit N from the above result (step 4) followed by the next 7 digits of convertStr.
            N = remainder.ToString() + convertStr.Substring(16, 7);
            // 6. Calculate N mod 97 = 24.
            remainder = int.Parse(N) % 97;
            // 7. Construct a new N from the above result (step 6) followed by the remaining 7 digits of convertStr.
            N = remainder.ToString() + convertStr.Substring(23, 7);
            // 8. Calculate N mod 97 = 1.
            remainder = int.Parse(N) % 97;
            // From step 8, the final result is convertStr mod 97 = 1 and the IBAN has passed this check digit test.
            // If the result is one, the IBAN corresponding to convertStr passes the check digit test.
            return remainder;
        }
    }
}

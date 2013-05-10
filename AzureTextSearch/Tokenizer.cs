using System;
using System.Collections.Generic;
using System.Linq;

namespace Two10.AzureTextSearch
{
    public static class Tokenizer
    {
        private static string[] stopWords = "a,able,about,across,after,all,almost,also,am,among,an,and,any,are,as,at,be,because,been,but,by,can,cannot,could,dear,did,do,does,either,else,ever,every,for,from,get,got,had,has,have,he,her,hers,him,his,how,however,i,if,in,into,is,it,its,just,least,let,like,likely,may,me,might,most,must,my,neither,no,nor,not,of,off,often,on,only,or,other,our,own,rather,said,say,says,she,should,since,so,some,than,that,the,their,them,then,there,these,they,this,tis,to,too,twas,us,wants,was,we,were,what,when,where,which,while,who,whom,why,will,with,would,yet,you,your".Split(',');


        public static IEnumerable<string> Tokenize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("input is empty");

            foreach (var token in input.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var cleanToken = StripUnwatedChars(token).ToLower();

                if (stopWords.Contains(cleanToken)) continue;
                if (cleanToken.Length <= 2) continue; // we don't want small words

                yield return cleanToken;
            }
        }

        private static string StripUnwatedChars(string value)
        {
            return new string(value.Where(c => !char.IsPunctuation(c)).ToArray());
        }

    }
}

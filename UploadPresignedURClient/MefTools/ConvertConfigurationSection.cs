using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolareshet.UploadPresignedURClient.MefTools
{
    class ConvertConfigurationSection
    {
        public static IDictionary<K, V> ConvertSection<K, V>(string sectionName)
        {
            Dictionary<K, V> dictionary = null;


            Hashtable hashTable = (Hashtable)ConfigurationManager.GetSection(sectionName);

            if (hashTable != null)
            {
                dictionary = new Dictionary<K, V>();

                foreach (var item in hashTable)
                {
                    dictionary.Add((K)((DictionaryEntry)item).Key, (V)((DictionaryEntry)item).Value);
                }
            }

            return dictionary;
        }
    }
}

using ClosedXML.Excel;
using GestaoSAC.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace GestaoSAC
{
    class Program
    {
        static List<string> IDsStores = new List<string>();
        static List<string> notFound = new List<string>();
        static Uri baseAddress = new Uri("https://api.sac.digital/v2/");
        static Authorization authorization;
        static Root root;
        static void Main(string[] args)
        {
            Application().Wait();
        }
        private static async Task Application() 
        {
            Console.Write("Client ID: ");
            string clientID = Console.ReadLine();
            Console.Write("Client Secrect: ");
            string clientSecrect = Console.ReadLine();
            Console.Write("Local Excel file (*.xlsx): ");
            string localFile = Console.ReadLine();
            List<string> stores = ReadExcel(localFile);
            await ConnectSACAsync(clientID, clientSecrect);
            foreach (string store in stores) await ContactFilter(store);
            foreach (string ID in IDsStores) await LinkGrupSACAsync(ID);
            Console.WriteLine("LOJAS QUE APRESENTARAM PROBLEMAS:");
            foreach (string storeNotFound in notFound) Console.WriteLine(storeNotFound);
        }
        private static List<string> ReadExcel(string localFile)
        {
            List<string> temp = new List<string>();
            XLWorkbook workbook = new XLWorkbook(localFile);
            IXLWorksheet worksheet = workbook.Worksheet(1);
            int currentRow = 2;
            while (true)
            {
                if (string.IsNullOrEmpty(worksheet.Cell($"A{currentRow}").Value.ToString())) break;
                temp.Add(worksheet.Cell($"A{currentRow}").Value.ToString());
                currentRow++;
            }
            return temp;
        }
        private static async Task LinkGrupSACAsync(string contactID) 
        {
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {authorization.Token}");
                using (var content = new StringContent($"{{  \"id\": \"MMpv\",  \"contact\": \"{contactID}\"}}", Encoding.Default, "application/json"))
                {
                    using (var response = await httpClient.PostAsync("client/groups/add", content))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseData);
                    }
                }
            }
        }
        private static async Task ContactFilter(string search, int filter = 1, int p = 1)
        {
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {authorization.Token}");
                using (var response = await httpClient.GetAsync($"client/contact/search?p={p}&filter={filter}&search={search}"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"---------------> {search}");
                    try
                    {
                        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(Root));
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseData));
                        root = (Root)js.ReadObject(ms);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    int contAux = 0;
                    List<int> removeItens = new List<int>();
                    foreach (Contact client in root.list)
                    {
                        if (!client.name.Contains(search))
                        {
                            removeItens.Add(contAux);
                        }
                        contAux++;
                    }
                    contAux = 0;
                    foreach (int position in removeItens)
                    {
                        if (contAux == 0)
                        {
                            root.list.RemoveAt(position);
                            contAux++;
                        }
                        else root.list.RemoveAt(position - contAux);
                    }
                    removeItens.Clear();
                    if (root.list.Count == 0) notFound.Add(search);
                    foreach (string id in TargetIDClients()) IDsStores.Add(id);
                }
            }
        }
        private static List<string> TargetIDClients()
        {
            List<string> auxIds = new List<string>();
            foreach (Contact client in root.list) auxIds.Add(client.id);
            return auxIds;
        }
        private static async Task ConnectSACAsync(string clientID, string clientSecrect)
        {
            using (HttpClient httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                using (StringContent content = new StringContent($"{{  \"client\": \"{clientID}\",  \"password\": \"{clientSecrect}\",  \"scopes\": [    \"manager\"  ]}}", Encoding.Default, "application/json"))
                {
                    using (var response = await httpClient.PostAsync("client/auth2/login", content))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        authorization = JsonConvert.DeserializeObject<Authorization>(responseData);
                        Console.WriteLine(responseData);
                    }
                }
            }
        }
    }
}

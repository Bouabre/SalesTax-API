using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using DomainObjects;
using IService;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Repository;
using Service;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace TaxJarConsoleApp
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            

            try
            {
                
                ITaxJarService service = new TaxJarService(new TaxJarRepository());




                HttpClient httpClient = new HttpClient();

                httpClient = setHttpClient(httpClient);
                GettotalTax(service, httpClient);
               GetCalculateTa(service, httpClient);
                GetLocalTaxRate(service, httpClient);



            }
            catch (Exception ex)
            {
              
                Console.ReadLine();
            }

        }
        public static HttpClient setHttpClient(HttpClient httpClient)
        {
            
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["URI"]);

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["apiKey"]);

            httpClient.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }


        public static void GetCalculateTa(ITaxJarService service , HttpClient httpClient)
        {
            string Uri = "v2/taxes";
            var taxesRequest = new TaxesRequest()
            {
                from_country = "US",
                from_zip = "92093",
                from_state = "CA",
                from_city = "La Jolla",
                from_street = "9500 Gilman Drive",
                to_country = "US",
                to_zip = "90002",
                to_state = "CA",
                to_city = "Los Angeles",
                to_street = "1335 E 103rd St",
                amount = 15,
                shipping = 1.5,
                nexus_addresses = new List<nexusAddresses>() {
    new nexusAddresses(){
      id = "Main Location",
      country = "US",
      zip = "92093",
      state = "CA",
      city = "La Jolla",
      street = "9500 Gilman Drive",
    }
  },
                line_items = new List<lineItems>(){
    new lineItems(){
      id = "1",
      quantity = 1,
      product_tax_code = "20010",
      unit_price = 15,
      discount = 0
    }
  }
            };

            var model = service.Calculate_the_taxes_for_an_order(httpClient, taxesRequest, Uri);

            Console.WriteLine("order_total_amount: {0} \t shipping: {1} \t taxable_amount: {2}", model.Result.tax.order_total_amount, model.Result.tax.shipping, model.Result.tax.taxable_amount);
        }
        public static void GetLocalTaxRate(ITaxJarService service, HttpClient httpClient)
        {
            string Zip = "90404";
            string Uri = "v2/rates/";

            var model = service.GettheTax_rates_for_a_location(httpClient, Zip, Uri);
            Console.WriteLine("city: {0} \t combined_rate: {1} \t state_rate: {2}", model.Result.rate.city, model.Result.rate.combined_rate, model.Result.rate.state_rate);
        }

        public static void GettotalTax(ITaxJarService service, HttpClient httpClient)
        {
            string Uri = "v2/taxes";
            var taxesRequest = new TaxesRequest()
            {
                from_country = "US",
                from_zip = "92093",
                from_state = "CA",
                from_city = "La Jolla",
                from_street = "9500 Gilman Drive",
                to_country = "US",
                to_zip = "90002",
                to_state = "CA",
                to_city = "Los Angeles",
                to_street = "1335 E 103rd St",
                amount = 15,
                shipping = 1.5,
                nexus_addresses = new List<nexusAddresses>() {
    new nexusAddresses(){
      id = "Main Location",
      country = "US",
      zip = "92093",
      state = "CA",
      city = "La Jolla",
      street = "9500 Gilman Drive",
    }
  },
                line_items = new List<lineItems>(){
    new lineItems(){
      id = "1",
      quantity = 1,
      product_tax_code = "20010",
      unit_price = 15,
      discount = 0
    }
  }
            };

            var model = service.TotalTaxTobecollected(httpClient, taxesRequest, Uri);

            Console.WriteLine("the total tax that needs to be collectedt: {0} ", model);
        }
    }
   

}

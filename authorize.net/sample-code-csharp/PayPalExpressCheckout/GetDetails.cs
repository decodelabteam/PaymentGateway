using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;

namespace net.authorize.sample
{
    public class PayPalGetDetails
    {
        public static ANetApiResponse Run(String ApiLoginID, String ApiTransactionKey, string TransactionID)
        {
            Console.WriteLine("PayPal Get Details Transaction");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };

            var payPalType = new payPalType
            {
                cancelUrl = "http://www.merchanteCommerceSite.com/Success/TC25262",
                successUrl = "http://www.merchanteCommerceSite.com/Success/TC25262",     // the url where the user will be returned to            
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = payPalType };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.getDetailsTransaction.ToString(),    // get the customer PayerID, email and shipping info
                payment         = paymentType,
                amount          = 19.45m,
                refTransId      = TransactionID
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if(response.transactionResponse.messages != null)
                    {
                        var shippingResponse = response.transactionResponse.shipTo;
                        Console.WriteLine("Shipping address : " + shippingResponse.address + ", " + shippingResponse.city + ", " + shippingResponse.state + ", " + shippingResponse.country);

                        if (response.transactionResponse.secureAcceptance != null)
                            Console.WriteLine("Payer ID : " + response.transactionResponse.secureAcceptance.PayerID);
                    }
                    else
                    {
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
            }

            return response;
        }
    }
}
